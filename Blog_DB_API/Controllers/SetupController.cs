using Blog_DB_API.Helpers;
using Blog_DB_API.Repository.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_DB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public SetupController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(new {status = 200, roles});
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            name = name.ToLower().Trim();
            if(await _roleManager.RoleExistsAsync(name))
            {
                return BadRequest(Responses.BadRequestResponse("This role alreaddy is exist"));
            }

            var role = await _roleManager.CreateAsync(new IdentityRole(name));
            if(!role.Succeeded)
            {
                return Ok(Responses.BadRequestResponse("role has been not added"));
            }

            return Ok(Responses.OkResponse($"role <<{name}>> has been added successfully"));
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(new { status = 200, users });
        }

        [HttpPost("addUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string rolename)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return BadRequest(Responses.BadRequestResponse("Invalid Email..."));

            if(!await _roleManager.RoleExistsAsync(rolename))
                return BadRequest(Responses.BadRequestResponse($"not exist any role by this name <<{rolename}>>..."));

            var addRoleToUser = await _userManager.AddToRoleAsync(user, rolename);

            if (addRoleToUser.Succeeded)
                return Ok(Responses.OkResponse("Adding the role to the user was completed successfully"));
            else
                return BadRequest(Responses.BadRequestResponse("Error!"));
        }

        [HttpGet("getUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return BadRequest(Responses.BadRequestResponse("User does not exist..."));

            var userRoles = await _userManager.GetRolesAsync(user);

            return Ok(new { status = 200, userRoles });
        }

        [HttpPost("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string rolename)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return BadRequest(Responses.BadRequestResponse("Invalid Email..."));

            if (!await _roleManager.RoleExistsAsync(rolename))
                return BadRequest(Responses.BadRequestResponse($"not exist any role by this name <<{rolename}>>..."));

            var removeRole = await _userManager.RemoveFromRoleAsync(user, rolename);

            if (removeRole.Succeeded)
                return Ok(Responses.OkResponse($"User {email} has been removed from role"));
            else
                return BadRequest(Responses.BadRequestResponse("Error!"));
        }
    }
}
