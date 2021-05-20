using System;
using System.Threading.Tasks;
using Netrunner.Shared.Internal;
using Netrunner.Shared.Services;
using WampSharp.V2;
using WampSharp.V2.Client;

namespace Netrunner.Auth
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string location = "ws://crossbar:8080/internal";
            const string realmName = "netrunner";

            DefaultWampChannelFactory channelFactory = new DefaultWampChannelFactory();

            IWampChannel channel = channelFactory.CreateJsonChannel(location, realmName, new WampInternalTicketAuthenticator());

            Task openTask = channel.Open();

            await openTask.ConfigureAwait(false);

            var authenticator = new Authenticator();

            IWampRealmProxy realm = channel.RealmProxy;

            Task<IAsyncDisposable> registrationTask = realm.Services.RegisterCallee(authenticator);

            await registrationTask.ConfigureAwait(false);

            // This line is required in order to release the WebSocket thread, otherwise it will be blocked by the Console.ReadLine() line.
            await Task.Yield();

            Console.ReadLine();
        }
    }
}