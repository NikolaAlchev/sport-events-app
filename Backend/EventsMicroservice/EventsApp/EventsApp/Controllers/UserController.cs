using Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/User/GetAllUsers
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<IdentityUser>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }

        // GET: api/User/GetUser/{id}
        [HttpGet("[action]/{id}")]
        public ActionResult<IdentityUser> GetUser(string id)
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
            var user = new IdentityUser
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
        [Authorize (Roles = "Admin")]
        public async Task<ActionResult> CreateAdmin([FromBody] CreateUserDto model)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }

            return BadRequest(result.Errors);
        }

        // PUT: api/User/UpdateUser/{id}
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
    }
}
