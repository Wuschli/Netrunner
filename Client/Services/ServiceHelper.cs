using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WampSharp.V2;
using WampSharp.V2.Client;

namespace Netrunner.Client.Services
{
    public interface IServiceHelper
    {
        Task<T> GetService<T>() where T : class;
        Task SetAuthToken(string? userId, string? token);
    }

    public class ServiceHelper : IServiceHelper
    {
        private readonly IConfiguration _config;
        private readonly Dictionary<Type, object> _proxyCache = new Dictionary<Type, object>();
        private IWampChannel? _channel;
        private IWampClientAuthenticator _authenticator = new DefaultWampClientAuthenticator();
        private string? _userId;
        private string? _token;

        public ServiceHelper(IConfiguration config)
        {
            _config = config;
        }

        public async Task<T> GetService<T>() where T : class
        {
            if (_proxyCache.TryGetValue(typeof(T), out var service) && service is T typedService)
                return typedService;
            if (_channel == null)
                await OpenChannel().ConfigureAwait(false);

            var proxy = _channel!.RealmProxy.Services.GetCalleeProxy<T>();
            _proxyCache.Add(typeof(T), proxy);
            return proxy;
        }

        public async Task SetAuthToken(string? userId, string? token)
        {
            Console.WriteLine($"Set new auth: {userId}, {token}");
            _userId = userId;
            _token = token;
            _proxyCache.Clear();
            await OpenChannel().ConfigureAwait(false);
        }

        private async Task OpenChannel()
        {
            if (_userId == null || _token == null)
                _authenticator = new DefaultWampClientAuthenticator();
            else
                _authenticator = new WampTicketAuthenticator(_userId, _token);
            if (_channel != null)
            {
                //TODO close channel
                //await _channel.Close("Auth Reconnect", new GoodbyeDetails {Message = "Auth Reconnect"}).ConfigureAwait(false);
                _channel = null;
            }

            var wampChannelFactory = new DefaultWampChannelFactory();
            _channel = wampChannelFactory.CreateJsonChannel(_config["wampAddress"], _config["wampRealm"], _authenticator);
            await _channel.Open().ConfigureAwait(false);
        }
    }
}