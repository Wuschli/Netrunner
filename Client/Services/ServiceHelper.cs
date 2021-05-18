using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WampSharp.V2;
using WampSharp.V2.Client;
using WampSharp.V2.Core.Contracts;

namespace Netrunner.Client.Services
{
    public interface IServiceHelper
    {
        Task<T> GetService<T>() where T : class;
        Task SetAuthToken(string? userName, string? token);
    }

    public class ServiceHelper : IServiceHelper
    {
        private readonly IConfiguration _config;
        private readonly Dictionary<Type, object> _proxyCache = new Dictionary<Type, object>();
        private IWampChannel? _channel;
        private IWampClientAuthenticator _authenticator = new DefaultWampClientAuthenticator();

        public ServiceHelper(IConfiguration config)
        {
            _config = config;
        }

        public async Task<T> GetService<T>() where T : class
        {
            if (_proxyCache.TryGetValue(typeof(T), out var service) && service is T typedService)
                return typedService;
            if (_channel == null)
            {
                var wampChannelFactory = new DefaultWampChannelFactory();
                _channel = wampChannelFactory.CreateJsonChannel(_config["wampAddress"], _config["wampRealm"], _authenticator);
                await _channel.Open();
            }

            var proxy = _channel.RealmProxy.Services.GetCalleeProxy<T>();
            _proxyCache.Add(typeof(T), proxy);
            return proxy;
        }

        public async Task SetAuthToken(string? userName, string? token)
        {
            _proxyCache.Clear();
            if (_channel != null)
                await _channel.Close("Auth Reconnect", new GoodbyeDetails {Message = "Auth Reconnect"});
            if (userName == null || token == null)
                _authenticator = new DefaultWampClientAuthenticator();
            else
                _authenticator = new WampTicketAuthenticator(userName, token);
        }
    }
}