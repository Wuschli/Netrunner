using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;

namespace Netrunner.Server
{
    public class Program
    {
        const string location = "ws://crossbar:8080/ws";
        const string realmName = "netrunner";
        private static IContainer Container { get; set; }

        public static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(type => type.IsDefined(typeof(WampServiceAttribute)))
                .AsSelf()
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

            Container = builder.Build();

            //var channelFactory = new DefaultWampChannelFactory();
            //var channel = channelFactory.CreateMsgpackChannel(location, realmName, new WampTicketAuthenticator());
            //await channel.Open().ConfigureAwait(false);
            //var realm = channel.RealmProxy;
            using (var scope = Container.BeginLifetimeScope())
            {
                var registeredTypes = scope.ComponentRegistry.Registrations.Select(r => r.Activator.LimitType);
                foreach (var type in registeredTypes)
                {
                    if (type.IsDefined(typeof(WampServiceAttribute), true))
                    {
                        var instance = scope.Resolve(type);
                        //await realm.Services.RegisterCallee(instance);
                    }
                }
            }

            cts.Token.WaitHandle.WaitOne();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class WampServiceAttribute : Attribute
    {
    }

    public class ServiceHost
    {

    }
}