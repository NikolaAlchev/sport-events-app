﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.DTO;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EventsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<EventsAppUser> _userManager;
        private readonly IConfiguration _configuration;
        public UserController(UserManager<EventsAppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // GET: api/User/GetAllUsers
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<EventsAppUser>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }

        // GET: api/User/GetUser/{id}
        [HttpGet("[action]/{id}")]
        public ActionResult<EventsAppUser> GetUser(string id)
        {
            var user = _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/User/CreateUser
        [HttpPost("[action]")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            var user = new EventsAppUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {

                var roleResult = await _userManager.AddToRoleAsync((EventsAppUser) user, "User");
                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }

            return BadRequest(result.Errors);
        }

        // POST: api/User/CreateAdmin
        [HttpPost("[action]")]
        [Authorize (Roles = "Admin")]
        public async Task<ActionResult> CreateAdmin([FromBody] CreateUserDto model)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync((EventsAppUser) user, model.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync((EventsAppUser) user, "Admin");
                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains("User"))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                {
                    return StatusCode(500, new { message = "Failed to assign user role" });
                }
            }

            var token = GenerateJwtToken(user);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,   // Cannot be accessed via JavaScript
                Secure = true,     // Only send over HTTPS
                Expires = DateTime.UtcNow.AddHours(2), // Set expiration
                SameSite= SameSiteMode.None
            };

            Response.Cookies.Append("jwt", token, cookieOptions);
            return Ok(new { message = "Logged in successfully" });
        }

        private string GenerateJwtToken(EventsAppUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("validate")]
        public IActionResult SomeProtectedEndpoint()
        {
            // Check if the cookie exists
            if (Request.Cookies.TryGetValue("jwt", out string token))
            {
                // Validate the token (e.g., decode JWT, check expiration, etc.)
                var isValid = ValidateToken(token);

                if (isValid)
                {
                    return Ok("You are authenticated");
                }
            }

            return Unauthorized("Not authenticated");
        }

        private bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!); // Same secret key as in GenerateJwtToken

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // Validate the signing key
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Signing key to validate against
                    ValidateIssuer = true, // Ensure the issuer matches
                    ValidIssuer = _configuration["JwtSettings:Issuer"], // Set issuer from appsettings.json
                    ValidateAudience = true, // Ensure the audience matches
                    ValidAudience = _configuration["JwtSettings:Audience"], // Set audience from appsettings.json
                    ValidateLifetime = true, // Ensure the token is not expired
                    ClockSkew = TimeSpan.Zero // No additional time for token expiration tolerance
                }, out SecurityToken validatedToken);

                // If we reach this point, the token is valid
                return true;
            }
            catch (SecurityTokenException)
            {
                // Token validation failed
                return false;
            }
        }
    }

/*    // PUT: api/User/UpdateUser/{id}
    [HttpPut("[action]/{id}")]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName ?? user.UserName;
            user.Email = model.Email ?? user.Email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(user);
            }

            return BadRequest(result.Errors);
        }

        // DELETE: api/User/DeleteUser/{id}
        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }
    }*/
}
