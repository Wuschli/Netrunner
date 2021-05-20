using System;
using System.Threading.Tasks;
using Netrunner.Shared.Auth;
using Netrunner.Shared.Services;
using WampSharp.V2.Core.Contracts;

namespace Netrunner.Auth
{
    internal class Authenticator : IAuthService, IAuthenticator
    {
        public async Task<AuthenticationResponse> Register(RegistrationRequest request)
        {
            throw new NotImplementedException();
            //var user = new ApplicationUser(request.UserName);
            //var result = await _userManager.CreateAsync(user, request.Password);
            //if (result.Succeeded)
            //{
            //    await _userManager.SignInAsync(user, false);
            //    return await AuthenticateAsync(user);
            //}

            //if (result.Errors != null)
            //    return new AuthenticationResponse
            //    {
            //        Error = string.Join(",", result.Errors.Select(error => error.Description)),
            //        Successful = false
            //    };

            //return new AuthenticationResponse
            //{
            //    Error = "Error!",
            //    Successful = false
            //};
        }

        public async Task<AuthenticationResponse> Login(LoginRequest request)
        {
            throw new NotImplementedException();
            //var result = await _userManager.PasswordSignInAsync(request.UserName, request.Password);
            //if (result.Succeeded)
            //{
            //    var user = _userManager.Users.Single(r => r.Username == request.UserName);
            //    return await AuthenticateAsync(user);
            //}

            //return new AuthenticationResponse
            //{
            //    Error = "Bad Credentials",
            //    Successful = false
            //};
        }

        public Task<AuthenticationResult> Authenticate(string realm, string authId, AuthenticationDetails details)
        {
            throw new WampException("netrunner.auth.invalid_ticket");
        }

        //private async Task<AuthenticationResponse> AuthenticateAsync(ApplicationUser user)
        //{
        //    var roles = await _userManager.GetRolesAsync(user);
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.Username),
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        //    };
        //    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        //    var jwtResult = _jwtAuthManager.GenerateTokens(user.Username, claims, DateTime.UtcNow);

        //    var response = new AuthenticationResponse
        //    {
        //        UserName = user.Username,
        //        AccessToken = jwtResult.AccessToken,
        //        RefreshToken = jwtResult.RefreshToken?.TokenString,
        //        Successful = true
        //    };
        //    return response;
        //}
    }
}