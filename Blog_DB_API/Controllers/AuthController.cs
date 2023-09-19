using Blog_DB_API.DTOs;
using Blog_DB_API.Helpers;
using Blog_DB_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Blog_DB_API.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        //register
        [HttpPost("register", Name = "CreateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            if (userDTO == null) return BadRequest(Responses.BadRequestResponse("Please enter your email and password"));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage);

                return BadRequest(new { Errors = errors });
            }

            //check user
            var user = await _userManager.FindByEmailAsync(userDTO.Email!);

            if (user is not null) return BadRequest(Responses.BadRequestResponse("This Email already exist, please another email"));

            //create user
            var newUser = new IdentityUser()
            {
                UserName = userDTO.Email,
                Email = userDTO.Email,
                EmailConfirmed = false
            };
            newUser.PasswordHash = _userManager.PasswordHasher.HashPassword(newUser, userDTO.Password!);

            var createUser = await _userManager.CreateAsync(newUser);

            if (!createUser.Succeeded) return BadRequest(Responses.BadRequestResponse("Server Error"));
            await _userManager.AddToRoleAsync(newUser, "user");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", new { token, email = newUser.Email }, Request.Scheme);
            var message = new Message(new string[] {newUser.Email!}, "Confirmation email link", confirmationLink);
            _emailSender.Send(message);  
            
            return Ok(Responses.OkResponse($"User Created & email sent to {newUser.Email} Successfully"));
        }

        //confirm email
        [HttpGet("ConfirmEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return BadRequest(Responses.BadRequestResponse("invalid email..."));
            var confirmEmail = await _userManager.ConfirmEmailAsync(user, token);

            if(!confirmEmail.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while checking email");
            }
            return Ok(Responses.OkResponse("Verification has been completed successfully"));
        }


        //login
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody]UserDTO userDTO)
        {
            if (userDTO == null) return BadRequest(Responses.BadRequestResponse("Please enter your email and password"));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage);

                return BadRequest(new { Errors = errors });
            }

            var user = await _userManager.FindByEmailAsync(userDTO.Email!);

            if (user is null) return BadRequest(Responses.BadRequestResponse("Invalid Email or Password..."));

            if (!user.EmailConfirmed)
            {
                return BadRequest(Responses.BadRequestResponse("Email needs to be confirmed"));
            }

            var checkPassword = await _userManager.CheckPasswordAsync(user, userDTO.Password!);

            if(!checkPassword) return BadRequest(Responses.BadRequestResponse("Invalid Email or Password..."));

            var token = await GenerateToken(user);
            return Ok(new { status = StatusCodes.Status200OK, token });
        }

        private async Task<List<Claim>> GetClaims(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //get claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            //get user roles
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            return claims;
        }

        //generate token
        private async Task<string> GenerateToken(IdentityUser user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Token").Value!);
            var claims = await GetClaims(user);
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(jwtTokenDescriptor);
            var jwt = jwtTokenHandler.WriteToken(token);
            return jwt;
        }
    }
}