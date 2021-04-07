using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Netrunner.Client.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        private AuthenticationState AnonymousState => new(new ClaimsPrincipal(new ClaimsIdentity()));

        public ApiAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>(AuthService.AuthTokenStorageKey);

            if (string.IsNullOrWhiteSpace(savedToken))
                return AnonymousState;

            try
            {
                var claims = ParseClaimsFromJwt(savedToken);

                var expiry = claims.FirstOrDefault(c => c.Type.Equals("exp"));
                if (expiry == null)
                    return AnonymousState;

                var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiry.Value));
                if (expiryDateTime.DateTime < DateTime.UtcNow)
                    return AnonymousState;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("Token was expired");
                return AnonymousState;
            }
        }

        public void MarkUserAsAuthenticated(string userName)
        {
            var authenticatedUser =
                new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, userName)}, "apiAuth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var claims = new List<Claim>();
            var serializer = new JsonNetSerializer();
            var dateTimeProvider = new UtcDateTimeProvider();
            var validator = new JwtValidator(serializer, dateTimeProvider);
            var algorithm = new HMACSHA256Algorithm();

            var payload = JwtBuilder.Create()
                .WithAlgorithm(algorithm)
                .WithValidator(validator)
                .Decode<IDictionary<string, object?>>(token);

            if (payload.TryGetValue(ClaimTypes.Role, out object? roles) && roles != null)
            {
                if (roles.ToString()!.Trim().StartsWith("["))
                {
                    var parsedRoles = JsonConvert.DeserializeObject<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
                }

                payload.Remove(ClaimTypes.Role);
            }

            claims.AddRange(payload.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty)));

            return claims;
        }
    }
}