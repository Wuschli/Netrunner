using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Netrunner.Shared.Identity;

namespace Netrunner.Client.Services
{
    public interface IAuthService
    {
        Task<string> AccessToken { get; }
        Task<AuthenticationResponse> Login(string userName, string password);
        Task Logout();
        Task<AuthenticationResponse> Register(string userName, string password);
    }

    public class AuthService : IAuthService
    {
        public const string AuthTokenStorageKey = "authToken";
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        public Task<string> AccessToken => _localStorage.GetItemAsync<string>(AuthTokenStorageKey).AsTask();


        public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<AuthenticationResponse> Register(string userName, string password)
        {
            var model = new RegistrationRequest
            {
                UserName = userName,
                Password = password
            };
            var response = await _httpClient.PostAsJsonAsync("api/v1/account/register", model);
            var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            if (!response.IsSuccessStatusCode)
                return result;

            await Authenticate(result);
            return result;
        }

        public async Task<AuthenticationResponse> Login(string userName, string password)
        {
            var model = new LoginRequest
            {
                UserName = userName,
                Password = password
            };
            var response = await _httpClient.PostAsJsonAsync("api/v1/account/Login", model);
            var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            if (!response.IsSuccessStatusCode)
                return result;

            await Authenticate(result);

            return result;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(AuthTokenStorageKey);
            ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private async Task Authenticate(AuthenticationResponse authentication)
        {
            if (!authentication.Successful || string.IsNullOrWhiteSpace(authentication.AccessToken))
                return;
            await _localStorage.SetItemAsync(AuthTokenStorageKey, authentication.AccessToken);
            ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsAuthenticated(authentication
                .UserName);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", authentication.AccessToken);
        }
    }
}