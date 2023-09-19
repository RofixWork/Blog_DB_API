using Blog_DB_API.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Blog_DB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsSetupController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ClaimsSetupController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("getAllClaims")]
        public async Task<IActionResult> GetAllClaims(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user is null)
                return BadRequest(Responses.BadRequestResponse("Invalid Email..."));

            var claims = await _userManager.GetClaimsAsync(user);
            return Ok(new { status = 200, claims });
        }

        [HttpPost]
        [Route("AddClaimToUser")]
        public async Task<IActionResult> AddClaimToUser([FromQuery]string email, string claimType, string claimValue)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return BadRequest(Responses.BadRequestResponse("Invalid Email..."));

            var addClaimsToUser = await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));

            if (addClaimsToUser.Succeeded)
            {
                return Ok(Responses.OkResponse("Claim has been added successfully"));
            }
            else
                return BadRequest(Responses.BadRequestResponse("Claim has not been added"));
        }
    }
}
