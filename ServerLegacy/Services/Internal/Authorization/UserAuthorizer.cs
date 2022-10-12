using System.Collections.Generic;
using System.Threading.Tasks;
using Netrunner.ServerLegacy.Attributes;
using Netrunner.Shared.Internal.Auth;
using WampSharp.V2.MetaApi;

namespace Netrunner.ServerLegacy.Services.Internal.Authorization
{
    [WampService]
    public class UserAuthorizer : IUserAuthorizer
    {
        private readonly List<AuthorizationRule> _rules = new();

        public UserAuthorizer(IUserManager userManager)
        {
            _rules.Add(new ChatRoomAuthorizer("^netrunner\\.chat\\.(?<roomId>[0-9a-f]+).*$", userManager));
            _rules.Add(new RoleAuthorizer("^netrunner\\.admin\\..*$", "admin", userManager));
        }

        public async Task<AuthorizationResult> AuthorizeUser(WampSessionDetails session, string uri, string action, Dictionary<string, object> options)
        {
            switch (action)
            {
                case "call":
                case "subscribe":
                    break;
                default:
                {
                    return new AuthorizationResult
                    {
                        Allow = false
                    };
                }
            }

            foreach (var rule in _rules)
            {
                if (!await rule.MatchAsync(uri, session))
                {
                    return new AuthorizationResult
                    {
                        Allow = false
                    };
                }
            }

            return new AuthorizationResult
            {
                Allow = true,
                Disclose = true
            };
        }
    }
}