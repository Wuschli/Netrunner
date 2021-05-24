using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Netrunner.Server.Attributes;
using Netrunner.Shared.Internal.Auth;
using Serilog;
using WampSharp.V2.MetaApi;

namespace Netrunner.Server.Services.Internal
{
    [WampService]
    public class UserAuthorizer : IUserAuthorizer
    {
        private readonly IUserManager _userManager;

        public UserAuthorizer(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<AuthorizationResult> AuthorizeUser(WampSessionDetails session, string uri, string action, Dictionary<string, object> options)
        {
            switch (action)
            {
                case "call":
                case "subscribe":
                    break;
                default:
                    return new AuthorizationResult
                    {
                        Allow = false
                    };
            }


            var regex = new Regex("^netrunner\\.chat\\.(?<roomId>[0-9a-f]+).*$", RegexOptions.IgnoreCase);
            var match = regex.Match(uri);
            if (match.Success)
            {
                var roomId = match.Groups["roomId"].Value;
                var user = await _userManager.GetUserAsync(session.AuthId);
                if (user == null)
                    return new AuthorizationResult();
                if (user.Rooms != null && user.Rooms.Contains(roomId))
                {
                    return new AuthorizationResult
                    {
                        Allow = true,
                        Disclose = true
                    };
                }

                return new AuthorizationResult();
            }

            return new AuthorizationResult
            {
                Allow = true,
                Disclose = true
            };
        }
    }
}