using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WampSharp.V2;

namespace Netrunner.Server.Services
{
    public abstract class WampServiceBase : IHostedService
    {
        const string location = "ws://crossbar:8080/ws";
        const string realmName = "netrunner";

        private IAsyncDisposable? _registration;

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            var channelFactory = new DefaultWampChannelFactory();
            var channel = channelFactory.CreateMsgpackChannel(location, realmName);
            await channel.Open().ConfigureAwait(false);
            var realm = channel.RealmProxy;
            _registration = await realm.Services.RegisterCallee(this);
            await Task.Yield();
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_registration != null)
            {
                await _registration.DisposeAsync();
                _registration = null;
            }
        }
    }
}