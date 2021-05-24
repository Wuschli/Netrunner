using System;
using WampSharp.V2.Authentication;
using WampSharp.V2.Client;
using WampSharp.V2.Core.Contracts;

namespace Netrunner.Shared.Internal
{
    public class WampInternalTicketAuthenticator : IWampClientAuthenticator
    {
        private static readonly string[] _authenticationMethods = {"ticket"};
        private const string _authenticationId = "internal";
        public string[] AuthenticationMethods => _authenticationMethods;

        public string AuthenticationId => _authenticationId;

        public AuthenticationResponse Authenticate(string authMethod, ChallengeDetails extra)
        {
            if (authMethod == "ticket")
            {
                Console.WriteLine("authenticating via '" + authMethod + "'");

                var ticket = Environment.GetEnvironmentVariable("WAMP_INTERNAL_TICKET");
                AuthenticationResponse result = new AuthenticationResponse {Signature = ticket};

                return result;
            }
            else
            {
                throw new WampAuthenticationException("don't know how to authenticate using '" + authMethod + "'");
            }
        }
    }
}