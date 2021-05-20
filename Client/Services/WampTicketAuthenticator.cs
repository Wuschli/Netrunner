﻿using System;
using WampSharp.V2.Authentication;
using WampSharp.V2.Client;
using WampSharp.V2.Core.Contracts;

namespace Netrunner.Client.Services
{
    public class WampTicketAuthenticator : IWampClientAuthenticator
    {
        private readonly string _username;
        private readonly string _token;
        private static readonly string[] _authenticationMethods = {"ticket"};

        public string[] AuthenticationMethods => _authenticationMethods;
        public string AuthenticationId => _username;

        public WampTicketAuthenticator(string username, string token)
        {
            _username = username;
            _token = token;
        }

        public AuthenticationResponse Authenticate(string authmethod, ChallengeDetails extra)
        {
            if (authmethod == "ticket")
            {
                Console.WriteLine("authenticating via '" + authmethod + "'");

                AuthenticationResponse result = new AuthenticationResponse {Signature = _token};

                return result;
            }
            else
            {
                throw new WampAuthenticationException("don't know how to authenticate using '" + authmethod + "'");
            }
        }
    }
}