﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Netrunner.Server.Configs;
using Netrunner.Server.Identity;
using Netrunner.Server.Identity.Data;
using Netrunner.Shared.Identity;
using Netrunner.Shared.Services;

namespace Netrunner.Server.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserManager _userManager;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly NetrunnerConfig _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserManager userManager, IJwtAuthManager jwtAuthManager, NetrunnerConfig config, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _jwtAuthManager = jwtAuthManager;
            _config = config;
            _logger = logger;
        }

        public async Task<AuthenticationResponse> Register(RegistrationRequest request)
        {
            var user = new ApplicationUser(request.UserName);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.SignInAsync(user, false);
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
            var result = await _userManager.PasswordSignInAsync(request.UserName, request.Password, false, false);
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