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
        Task<AuthenticationResponse> Login(LoginRequest login);
        Task Logout();
        Task<AuthenticationResponse> Register(RegistrationRequest registration);
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<AuthenticationResponse> Register(RegistrationRequest registration)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/account/register", registration);
            var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
            await Authenticate(result);
            return result;
        }

        public async Task<AuthenticationResponse> Login(LoginRequest login)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/account/Login", login);
            var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            if (!response.IsSuccessStatusCode)
                return result;

            await Authenticate(result);

            return result;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private async Task Authenticate(AuthenticationResponse authentication)
        {
            await _localStorage.SetItemAsync("authToken", authentication.AccessToken);
            ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsAuthenticated(authentication.UserName);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authentication.AccessToken);
        }
    }
}