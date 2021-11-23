using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using JWT.Serializers;
using Microsoft.AspNetCore.Components.Authorization;
using Netrunner.Shared.Auth;
using Netrunner.Shared.Internal.Auth;

namespace Netrunner.Client.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IServiceHelper _serviceHelper;

        private AuthenticationState AnonymousState => new(new ClaimsPrincipal(new ClaimsIdentity()));

        public ApiAuthenticationStateProvider(ILocalStorageService localStorage, IServiceHelper serviceHelper)
        {
            _localStorage = localStorage;
            _serviceHelper = serviceHelper;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>(AuthHelper.AuthTokenStorageKey);

            if (string.IsNullOrWhiteSpace(savedToken))
                return AnonymousState;

            try
            {
                var payload = ParseJwt(savedToken);
                if (payload.Expiration == null)
                    return AnonymousState;

                var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(payload.Expiration.Value);
                if (expiryDateTime.DateTime < DateTime.UtcNow)
                    return AnonymousState;

                await _serviceHelper.SetAuthToken(payload.UserId, savedToken);
                var claims = GetClaimsFromTokenPayload(payload);

                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token was expired");
                return AnonymousState;
            }
        }

        public void MarkUserAsAuthenticated(AuthenticationResponse authentication)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, authentication.AuthenticationId)
            };

            foreach (var role in authentication.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "apiAuth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private TokenPayload ParseJwt(string token)
        {
            var serializer = new JsonNetSerializer();
            var dateTimeProvider = new UtcDateTimeProvider();
            var validator = new JwtValidator(serializer, dateTimeProvider);
            var algorithm = new HMACSHA256Algorithm();

            var payload = JwtBuilder.Create()
                .WithAlgorithm(algorithm)
                .WithValidator(validator)
                .Decode<TokenPayload>(token);
            return payload;
        }

        private IEnumerable<Claim> GetClaimsFromTokenPayload(TokenPayload payload)
        {
            if (payload.Username != null)
                yield return new Claim("name", payload.Username);
            if (payload.UserId != null)
                yield return new Claim("identifier", payload.UserId);
            if (payload.Roles != null)
                foreach (var role in payload.Roles)
                    yield return new Claim("role", role);
            if (payload.Expiration != null)
                yield return new Claim("exp", payload.Expiration.Value.ToString());
            if (payload.IssuedAt != null)
                yield return new Claim("iat", payload.IssuedAt.Value.ToString());
            if (payload.Subject != null)
                yield return new Claim("sub", payload.Subject);
            if (payload.Issuer != null)
                yield return new Claim("iss", payload.Issuer);
            if (payload.Audience != null)
                yield return new Claim("aud", payload.Audience);
            if (payload.NotBefore != null)
                yield return new Claim("nbf", payload.NotBefore);
            if (payload.TokenId != null)
                yield return new Claim("jti", payload.TokenId);
            if (payload.SessionId != null)
                yield return new Claim("sid", payload.SessionId);
        }
    }
}