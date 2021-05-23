using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Netrunner.Shared.Internal.Auth;
using NUnit.Framework;

namespace Netrunner.Auth.Test
{
    public class GenerateTokenTests
    {
        private JwtAuthManager _authManager;

        [SetUp]
        public void SetUp()
        {
            var config = new Config
            {
                Jwt = new JwtConfig
                {
                    Secret = "KEY1312312312RANDO@SDKFSDJNWOERE",
                    Issuer = "Netrunner",
                    Audience = "Netrunner",
                    AccessTokenExpiration = 30,
                    RefreshTokenExpiration = 30
                }
            };
            _authManager = new JwtAuthManager(config);
        }

        [Test]
        public async Task TestJwtTokenGeneration()
        {
            var payload = new TokenPayload
            {
                Username = "TestName",
                UserId = "TestId",
                Roles = new List<string> {"Role1, Role2"}
            };
            var result = await _authManager.GenerateTokens("TestId", payload, DateTimeOffset.UtcNow);
            var token = result.AccessToken;
            Assert.NotNull(token);
        }
    }
}