using System;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Netrunner.Server.Attributes;
using Netrunner.Server.Configs;

namespace Netrunner.Server
{
    public class Program
    {
        private const string ConfigName = "Netrunner";

        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            var configRoot = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Development.json", true)
                .Build();

            var config = configRoot.GetSection(ConfigName).Get<NetrunnerConfig>();
            builder.RegisterInstance<IConfiguration>(configRoot);
            builder.RegisterInstance(config);

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(type => type.IsDefined(typeof(WampServiceAttribute)))
                .AsSelf()
                .AsImplementedInterfaces();

            var container = builder.Build();
            var serviceHost = new ServiceHost();
            serviceHost.Run(container);
        }
    }
}