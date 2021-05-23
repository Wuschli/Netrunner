using System;
using WampSharp.V2.Authentication;
using WampSharp.V2.Client;
using WampSharp.V2.Core.Contracts;

namespace Netrunner.Client.Services
{
    public class WampTicketAuthenticator : IWampClientAuthenticator
    {
        private readonly string _token;
        private static readonly string[] _authenticationMethods = {"ticket"};

        public string[] AuthenticationMethods => _authenticationMethods;
        public string AuthenticationId { get; }

        public WampTicketAuthenticator(string authenticationId, string token)
        {
            AuthenticationId = authenticationId;
            _token = token;
        }

        public AuthenticationResponse Authenticate(string authMethod, ChallengeDetails extra)
        {
            if (authMethod == "ticket")
            {
                Console.WriteLine("authenticating via '" + authMethod + "'");

                AuthenticationResponse result = new AuthenticationResponse {Signature = _token};

                return result;
            }
            else
            {
                throw new WampAuthenticationException("don't know how to authenticate using '" + authMethod + "'");
            }
        }
    }
}