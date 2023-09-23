using demerge_blog_API.DTOs;
using demerge_blog_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using static demerge_blog_API.Helpers.Responses;
using demerge_blog_API.Services;

namespace demerge_blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        public AuthController(UserManager<User> userManager, IConfiguration configuration, IEmailSender emailSender)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        //register
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            //check user email
            if(await _userManager.FindByEmailAsync(userDTO.Email!) is not null)
            {
                return BadRequest(BadRequestResponse($"This email <<{userDTO.Email}>> already Exist, please enter another email"));
            }
            //create a new user
            User user = new()
            {
                UserName = userDTO.Email!.ToLower(),
                Email = userDTO.Email.ToLower(),
                EmailConfirmed = false
            };
            //hash password
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userDTO.Password!);
            //create user
            var isCreated = await _userManager.CreateAsync(user);
            //created successfully
            if (!isCreated.Succeeded)
            {
                return BadRequest(BadRequestResponse("User has been not registered"));
            }
            //add role user by default
            await _userManager.AddToRoleAsync(user, "user");
            //create token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //create confirmation link
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", new { userId = user.Id, token }, Request.Scheme);
            var body = $"<a href=\"{confirmationLink}\">Click to confirm your Email</a>";
            //create email message
            var message = new Message(user.Email, "Confirmation Email", body);
            //send email
            _emailSender.SendEmail(message);
            return Ok(OKtResponse($"Please Confirm your email Address"));
        }

        //confirm email
        [HttpGet("ConfirmEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            //check url params
            if (userId == null || token == null)
                return BadRequest(BadRequestResponse("Invalid url parameters"));
            //find user
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return BadRequest(BadRequestResponse("invalid user Id param"));
            //confirm email 
            var confirmEmail = await _userManager.ConfirmEmailAsync(user, token);

            if (confirmEmail.Succeeded)
                return Ok(OKtResponse("Email confirmation Succeeded"));

            return BadRequest(BadRequestResponse("Email confirmation is invalid"));
        }

        //login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDTO userDTO)
        {
            //find user by email
            var user = await _userManager.FindByEmailAsync(userDTO.Email!);
            //check user email
            if (user is null)
                return BadRequest(BadRequestResponse($"Invalid Email OR Password"));

            //check passwrod
            if (!await _userManager.CheckPasswordAsync(user, userDTO.Password!))
                return BadRequest(BadRequestResponse($"Invalid Email OR Password"));

            if (!user.EmailConfirmed)
            {
                return BadRequest(BadRequestResponse("This Email need Confirmation"));
            }

            var token = await GenerateToken(user);
            return Ok(new
            {
                status = 200,
                token
            });
        }
        //claim
        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim("Id", user.Id!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            return claims;
        }

        //token
        private async Task<string> GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value!);
            var jwtTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(await GetClaims(user)),
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(jwtTokenDescriptor);
            var jwt = jwtTokenHandler.WriteToken(token);
            return jwt;
        }
    }
}
