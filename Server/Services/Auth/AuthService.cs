using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Netrunner.Server.Attributes;
using Netrunner.Server.Identity;
using Netrunner.Server.Identity.Data;
using Netrunner.Shared.Identity;
using Netrunner.Shared.Services;

namespace Netrunner.Server.Services.Auth
{
    [WampService]
    public class AuthService : IAuthService
    {
        private readonly IUserManager _userManager;
        private readonly IJwtAuthManager _jwtAuthManager;

        public AuthService(IUserManager userManager, IJwtAuthManager jwtAuthManager)
        {
            _userManager = userManager;
            _jwtAuthManager = jwtAuthManager;
        }

        public async Task<AuthenticationResponse> Register(RegistrationRequest request)
        {
            var user = new ApplicationUser(request.UserName);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.SignInAsync(user, false);
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
            var result = await _userManager.PasswordSignInAsync(request.UserName, request.Password);
            if (result.Succeeded)
            {
                var user = _userManager.Users.Single(r => r.Username == request.UserName);
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
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtResult = _jwtAuthManager.GenerateTokens(user.Username, claims, DateTime.UtcNow);

            var response = new AuthenticationResponse
            {
                UserName = user.Username,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken?.TokenString,
                Successful = true
            };
            return response;
        }
    }
}