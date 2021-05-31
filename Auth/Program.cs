﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Netrunner.Shared.Internal;
using Serilog;
using WampSharp.V2;
using WampSharp.V2.Client;

namespace Netrunner.Auth
{
    class Program
    {
        const string Location = "ws://crossbar:8080/internal";
        const string RealmName = "netrunner";
        const string ConfigName = "Authenticator";

        static async Task Main(string[] args)
        {
            var configRoot = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Development.json", true)
                .Build();
            var config = configRoot.GetSection(ConfigName).Get<Config>();

            using var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            DefaultWampChannelFactory channelFactory = new DefaultWampChannelFactory();

            IWampChannel channel = channelFactory.CreateJsonChannel(Location, RealmName, new WampInternalTicketAuthenticator());

            for (var i = 0; i < 100; i++)
            {
                try
                {
                    await channel.Open().ConfigureAwait(false);
                    break;
                }
                catch (Exception e)
                {
                    var delay = 3000;
                    log.Warning(e, $"Failed to connect to Router. Trying again in {delay}ms");
                    await Task.Delay(delay);
                }
            }

            var authenticator = new Authenticator(config);

            IWampRealmProxy realm = channel.RealmProxy;

            Task<IAsyncDisposable> registrationTask = realm.Services.RegisterCallee(authenticator);

            await registrationTask.ConfigureAwait(false);

            // This line is required in order to release the WebSocket thread, otherwise it will be blocked by the Console.ReadLine() line.
            await Task.Yield();

            Console.ReadLine();
        }
    }
}