using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.DTO;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interface;

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

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<EventsAppUser>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
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

                var roleResult = await _userManager.AddToRoleAsync(user, "User");
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateAdmin([FromBody] CreateUserDto model)
        {
            var user = new EventsAppUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync((EventsAppUser)user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync((EventsAppUser)user, "Admin");
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
            var token = await GenerateJwtToken(user);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(2),
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("jwt", token, cookieOptions);
            return Ok(new { message = "Logged in successfully" });
        }

        public async Task<string> GenerateJwtToken(EventsAppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
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
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("validate")]
        public IActionResult Validation()
        {
            if (Request.Cookies.TryGetValue("jwt", out string token))
            {
                if (ValidateToken(token, out ClaimsPrincipal principal))
                {
                    var username = principal.FindFirst(ClaimTypes.Name)?.Value;
                    return Ok(username);
                }
            }
            return Unauthorized("Not authenticated");
        }


        [HttpGet("is-admin")]
        public IActionResult isAdmin()
        {
            if (Request.Cookies.TryGetValue("jwt", out string token))
            {
                if (ValidateToken(token, out ClaimsPrincipal principal))
                {
                    var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
                    return Ok(roles.Contains("Admin"));
                }
            }
            return Unauthorized("Not authenticated");
        }


        private bool ValidateToken(string token, out ClaimsPrincipal principal)
        {
            principal = null;

            if (string.IsNullOrEmpty(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!);

            try
            {
                principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        // POST: api/User/Logout
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("jwt", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(-1),
                SameSite = SameSiteMode.None
            });

            return Ok(new { message = "Logged out successfully" });
        }

    }

}
