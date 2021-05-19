using Microsoft.Extensions.DependencyInjection;
using Netrunner.Server.Services;

namespace Netrunner.Server
{
    public static class ServiceCollectionExtensions
    {
        //public static void AddFactory<TService, TImplementation>(this IServiceCollection services)
        //    where TService : class
        //    where TImplementation : class, TService
        //{
        //    services.AddTransient<TService, TImplementation>();
        //    services.AddSingleton<Func<TService>>(x => () =>
        //    {
        //        try
        //        {
        //            return x.GetService<TService>();
        //        }
        //        catch (Exception e)
        //        {
        //            x.GetService<ILogger<TService>>().LogError(e, "Error in Factory");
        //            throw;
        //        }
        //    });
        //}
        public static void AddWampService<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddScoped<TService, TImplementation>();
            services.AddHostedService<WampHostedService<TService>>();
        }
    }
}