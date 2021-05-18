using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Netrunner.Server.Configs;
using Netrunner.Server.Identity;
using Netrunner.Server.Identity.Data;
using Netrunner.Shared.Identity;
using Netrunner.Shared.Services;

namespace Netrunner.Server.Services.Auth
{
    public class AuthService : WampServiceBase, IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly NetrunnerConfig _config;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtAuthManager jwtAuthManager, NetrunnerConfig config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtAuthManager = jwtAuthManager;
            _config = config;
        }

        public async Task<AuthenticationResponse> Register(RegistrationRequest request)
        {
            var user = new ApplicationUser(request.UserName);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                //var token = AuthenticationHelper.GenerateJwtToken(user, _config);
                //var response = new RegistrationResponse {UserName = user.UserName, AccessToken = token};
                return await AuthenticateAsync(user);
            }

            if (result.Errors != null)
                return new AuthenticationResponse
                {
                    Error = string.Join(",", result.Errors.Select(error => error.Description)),
                    Successful = false
                };

            return new AuthenticationResponse
            {
                Error = "Error!",
                Successful = false
            };
        }

        public async Task<AuthenticationResponse> Login(LoginRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, false, false);
            if (result.Succeeded)
            {
                var user = _userManager.Users.Single(r => r.UserName == request.UserName);
                return await AuthenticateAsync(user);
            }

            return new AuthenticationResponse
            {
                Error = "Bad Credentials",
                Successful = false
            };
        }

        private async Task<AuthenticationResponse> AuthenticateAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtResult = _jwtAuthManager.GenerateTokens(user.UserName, claims, DateTime.UtcNow);

            var response = new AuthenticationResponse
            {
                UserName = user.UserName,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken?.TokenString,
                Successful = true
            };
            return response;
        }
    }
}