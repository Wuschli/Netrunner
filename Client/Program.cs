using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Netrunner.Client.Services;
using Netrunner.Shared.Services;
using WampSharp.V2;
using IAuthService = Netrunner.Shared.Services.IAuthService;

namespace Netrunner.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            builder.Services.AddScoped<IAuthHelper, AuthHelper>();
            builder.Services.AddSingleton<IServiceHelper, ServiceHelper>();

            await builder.Build().RunAsync();
        }
    }
}