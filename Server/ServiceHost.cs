using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Netrunner.Server.Attributes;
using Netrunner.Shared.Internal;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;

namespace Netrunner.Server
{
    public class ServiceHost
    {
        private readonly List<Task> _tasks = new();

        public void Run(ILifetimeScope container)
        {
            var cts = new CancellationTokenSource();
            _tasks.Add(RunWampServices(container, cts.Token));

            Task.WaitAll(_tasks.ToArray(), cts.Token);
        }

        private async Task RunWampServices(ILifetimeScope container, CancellationToken cancellationToken)
        {
            var wampClientService = container.Resolve<IWampClientService>();
            var realm = await wampClientService.GetRealmAsync();
            //var hostedServices = new List<object>();

            var registerOptions = new RegisterOptions
            {
                DiscloseCaller = true
            };

            await using (var scope = container.BeginLifetimeScope())
            {
                var registeredTypes = scope.ComponentRegistry.Registrations.Select(r => r.Activator.LimitType);
                foreach (var type in registeredTypes.Where(t => t.IsDefined(typeof(WampServiceAttribute), true)).Distinct())
                {
                    var instance = scope.Resolve(type);
                    //hostedServices.Add(instance);
                    await realm.Services.RegisterCallee(instance, new CalleeRegistrationInterceptor(registerOptions)).ConfigureAwait(false);
                    await Task.Yield();
                }
            }

            await cancellationToken;
        }
    }
}