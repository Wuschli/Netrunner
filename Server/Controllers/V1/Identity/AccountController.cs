using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Netrunner.Server.Helpers;
using Netrunner.Server.Identity.Data;
using Netrunner.Server.Models;
using Netrunner.Shared.Identity;

namespace Netrunner.Server.Controllers.V1.Identity
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtSettings _jwtSettings;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtSettings jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RegistrationResponse>> Register([FromBody] RegistrationRequest request)
        {
            var user = new ApplicationUser(request.UserName);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                var token = AuthenticationHelper.GenerateJwtToken(user, _jwtSettings);
                var response = new RegistrationResponse {UserName = user.UserName, Token = token};
                return Created("api/v1/account/register", response);
            }

            if (result.Errors != null)
                return BadRequest(string.Join(",", result.Errors.Select(error => error.Description)));
            return BadRequest("Error!");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, false, false);
            if (result.Succeeded)
            {
                var user = _userManager.Users.Single(r => r.UserName == request.UserName);
                var token = AuthenticationHelper.GenerateJwtToken(user, _jwtSettings);

                var response = new LoginResponse
                {
                    UserName = user.UserName,
                    Token = token
                };
                return Ok(response);
            }

            return Unauthorized("Bad Credentials");
        }
    }
}