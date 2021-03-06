using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Netrunner.Shared.Auth;
using Netrunner.Shared.Services;

namespace Netrunner.Client.Services
{
    public interface IAuthHelper
    {
        Task<string> AccessToken { get; }
        Task<AuthenticationResponse?> Login(string? username, string? password);
        Task Logout();
        Task<AuthenticationResponse?> Register(string? username, string? password);
    }

    public class AuthHelper : IAuthHelper
    {
        public const string AuthTokenStorageKey = "authToken";
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly IServiceHelper _serviceHelper;
        public Task<string> AccessToken => _localStorage.GetItemAsync<string>(AuthTokenStorageKey).AsTask();


        public AuthHelper(AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage, IServiceHelper serviceHelper)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            _serviceHelper = serviceHelper;
        }

        public async Task<AuthenticationResponse?> Register(string? username, string? password)
        {
            var model = new RegistrationRequest
            {
                Username = username,
                Password = password
            };
            var authService = await _serviceHelper.GetService<IAuthService>();
            var response = await authService.Register(model);

            if (!response.Successful)
                return response;

            await Authenticate(response);
            return response;
        }

        public async Task<AuthenticationResponse?> Login(string? username, string? password)
        {
            var model = new LoginRequest
            {
                Username = username,
                Password = password
            };
            var authService = await _serviceHelper.GetService<IAuthService>();
            var response = await authService.Login(model);

            if (!response.Successful)
                return response;

            await Authenticate(response);

            return response;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(AuthTokenStorageKey);
            ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsLoggedOut();
            await _serviceHelper.SetAuthToken(null, null);
        }

        private async Task Authenticate(AuthenticationResponse? authentication)
        {
            Console.WriteLine("Authenticating...");
            if (authentication == null || !authentication.Successful || string.IsNullOrWhiteSpace(authentication.AccessToken))
                return;
            await _localStorage.SetItemAsync(AuthTokenStorageKey, authentication.AccessToken);
            await _serviceHelper.SetAuthToken(authentication.AuthenticationId, authentication.AccessToken);
            ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsAuthenticated(authentication.AuthenticationId);
        }
    }
}