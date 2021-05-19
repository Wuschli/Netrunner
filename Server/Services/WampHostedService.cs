using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WampSharp.V2;

namespace Netrunner.Server.Services
{
    public sealed class WampHostedService<TService> : IHostedService where TService : class
    {
        private readonly IServiceProvider _serviceProvider;
        const string location = "ws://crossbar:8080/ws";
        const string realmName = "netrunner";

        private IAsyncDisposable? _registration;
        private IServiceScope? _scope;

        public WampHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var channelFactory = new DefaultWampChannelFactory();
            var channel = channelFactory.CreateMsgpackChannel(location, realmName, new WampTicketAuthenticator());
            await channel.Open().ConfigureAwait(false);
            var realm = channel.RealmProxy;
            _scope = _serviceProvider.CreateScope();
            var instance = _scope.ServiceProvider.GetRequiredService<TService>();
            _registration = await realm.Services.RegisterCallee(instance);
            await Task.Yield();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_registration != null)
            {
                await _registration.DisposeAsync();
                _registration = null;
            }

            if (_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }
        }
    }
}