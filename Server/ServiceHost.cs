using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Netrunner.Server.Attributes;
using Netrunner.Server.Services;
using WampSharp.V2;

namespace Netrunner.Server
{
    public class ServiceHost
    {
        const string location = "ws://crossbar:8080/ws";
        const string realmName = "netrunner";

        private readonly List<object> _hostedServices = new();
        private readonly List<Task> _tasks = new();

        public void Run(ILifetimeScope container)
        {
            var cts = new CancellationTokenSource();
            _tasks.Add(RunWampServices(container, cts.Token));

            Task.WaitAll(_tasks.ToArray(), cts.Token);
        }

        private async Task RunWampServices(ILifetimeScope container, CancellationToken cancellationToken)
        {
            var channelFactory = new DefaultWampChannelFactory();
            var channel = channelFactory.CreateMsgpackChannel(location, realmName, new WampTicketAuthenticator());
            await channel.Open().ConfigureAwait(false);
            var realm = channel.RealmProxy;
            await using (var scope = container.BeginLifetimeScope())
            {
                var registeredTypes = scope.ComponentRegistry.Registrations.Select(r => r.Activator.LimitType);
                foreach (var type in registeredTypes.Where(t => t.IsDefined(typeof(WampServiceAttribute), true)).Distinct())
                {
                    var instance = scope.Resolve(type);
                    _hostedServices.Add(instance);
                    await realm.Services.RegisterCallee(instance);
                }
            }

            await cancellationToken;
        }
    }
}