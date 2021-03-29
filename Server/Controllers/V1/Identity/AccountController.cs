using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Netrunner.Server.Identity;
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
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IJwtSettings _jwtSettings;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtAuthManager jwtAuthManager, IJwtSettings jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtAuthManager = jwtAuthManager;
            _jwtSettings = jwtSettings;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponse>> Register([FromBody] RegistrationRequest request)
        {
            var user = new ApplicationUser(request.UserName);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                //var token = AuthenticationHelper.GenerateJwtToken(user, _jwtSettings);
                //var response = new RegistrationResponse {UserName = user.UserName, AccessToken = token};
                return Created("api/v1/account/register", await AuthenticateAsync(user));
            }

            if (result.Errors != null)
                return BadRequest(new AuthenticationResponse
                {
                    Error = string.Join(",", result.Errors.Select(error => error.Description)),
                    Successful = false
                });

            return BadRequest(new AuthenticationResponse
            {
                Error = "Error!",
                Successful = false
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, false, false);
            if (result.Succeeded)
            {
                var user = _userManager.Users.Single(r => r.UserName == request.UserName);
                return Ok(await AuthenticateAsync(user));
            }

            return Unauthorized(new AuthenticationResponse
            {
                Error = "Bad Credentials",
                Successful = false
            });
        }

        private async Task<AuthenticationResponse> AuthenticateAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, string.Join(",", roles))
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(user.UserName, claims, DateTime.Now);
            //var token = AuthenticationHelper.GenerateJwtToken(user, _jwtSettings);

            var response = new AuthenticationResponse
            {
                UserName = user.UserName,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString,
                Successful = true
            };
            return response;
        }
    }
}