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

namespace Netrunner.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // WAMP
            var wampChannelFactory = new DefaultWampChannelFactory();
            var wampChannel = wampChannelFactory.CreateJsonChannel(builder.Configuration["wampAddress"], builder.Configuration["wampRealm"]);
            await wampChannel.Open();

            builder.Services.AddSingleton(wampChannel.RealmProxy.Services.GetCalleeProxy<IPingService>());

            await builder.Build().RunAsync();
        }
    }
}