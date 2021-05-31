using System;
using System.Threading.Tasks;
using Netrunner.Shared.Internal;
using Serilog;
using WampSharp.V2;
using WampSharp.V2.Client;

namespace Netrunner.Server
{
    public interface IWampClientService
    {
        Task<IWampRealmProxy> GetRealmAsync();
        Task PublishAsync<T>(string topic, T value);
    }

    public class WampClientService : IWampClientService
    {
        const string Location = "ws://crossbar:8080/internal";
        const string RealmName = "netrunner";

        private IWampChannel? _channel;

        public async Task<IWampRealmProxy> GetRealmAsync()
        {
            var channelFactory = new DefaultWampChannelFactory();
            if (_channel == null)
            {
                _channel = channelFactory.CreateJsonChannel(Location, RealmName, new WampInternalTicketAuthenticator());
                for (var i = 0; i < 100; i++)
                {
                    try
                    {
                        await _channel.Open().ConfigureAwait(false);
                        break;
                    }
                    catch (Exception e)
                    {
                        var delay = 3000;
                        Log.Warning(e, $"Failed to connect to Router. Trying again in {delay}ms");
                        await Task.Delay(delay);
                    }
                }
            }

            await Task.Yield();
            return _channel.RealmProxy;
        }

        public async Task PublishAsync<T>(string topic, T value)
        {
            var realm = await GetRealmAsync();
            var subject = realm.Services.GetSubject<T>(topic);
            subject.OnNext(value);
        }
    }
}