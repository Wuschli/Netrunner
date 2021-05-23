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
        Task SetAuthToken(string? userId, string? token);
        Task<Guid> Subscribe<T>(string topic, Action<T> action);
        void Unsubscribe(Guid subscriptionId);
    }

    public class ServiceHelper : IServiceHelper
    {
        private readonly IConfiguration _config;
        private readonly Dictionary<Type, object> _proxyCache = new Dictionary<Type, object>();
        private readonly Dictionary<Guid, IWampSubscriptionWrapper> _subscriptions = new Dictionary<Guid, IWampSubscriptionWrapper>();

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

        public async Task<Guid> Subscribe<T>(string topic, Action<T> subscriber)
        {
            if (_channel == null)
                await OpenChannel().ConfigureAwait(false);
            var wrapper = new WampSubscriptionWrapper<T>(topic, subscriber);
            wrapper.Subscribe(_channel!.RealmProxy);
            _subscriptions.Add(wrapper.Id, wrapper);
            return wrapper.Id;
        }

        public void Unsubscribe(Guid subscriptionId)
        {
            if (_subscriptions.TryGetValue(subscriptionId, out var wrapper))
            {
                wrapper.Dispose();
                _subscriptions.Remove(subscriptionId);
            }
        }

        private async Task OpenChannel()
        {
            if (_userId == null || _token == null)
                _authenticator = new DefaultWampClientAuthenticator();
            else
                _authenticator = new WampTicketAuthenticator(_userId, _token);

            foreach (var sub in _subscriptions.Values)
                sub.Dispose();
            var oldChannel = _channel;
            _channel = null;

            var wampChannelFactory = new DefaultWampChannelFactory();
            _channel = wampChannelFactory.CreateJsonChannel(_config["wampAddress"], _config["wampRealm"], _authenticator);
            await _channel.Open().ConfigureAwait(false);

            foreach (var sub in _subscriptions.Values)
                sub.Subscribe(_channel.RealmProxy);

            if (oldChannel != null)
            {
                try
                {
                    await oldChannel.Close("Auth Reconnect", new GoodbyeDetails {Message = "Auth Reconnect"});
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class WampSubscriptionWrapper<T> : IWampSubscriptionWrapper
        {
            private readonly string _topic;
            private readonly Action<T> _subscriber;

            private IDisposable? _subscription;

            public Guid Id { get; }

            public WampSubscriptionWrapper(string topic, Action<T> subscriber)
            {
                _topic = topic;
                _subscriber = subscriber;
                Id = Guid.NewGuid();
            }

            public void Subscribe(IWampRealmProxy realmProxy)
            {
                if (_subscription != null)
                    Dispose();
                var subject = realmProxy.Services.GetSubject<T>(_topic);
                _subscription = subject.Subscribe(_subscriber);
            }

            public void Dispose()
            {
                _subscription?.Dispose();
                _subscription = null;
            }
        }
    }

    public interface IWampSubscriptionWrapper
    {
        Guid Id { get; }
        void Subscribe(IWampRealmProxy realmProxy);
        void Dispose();
    }
}