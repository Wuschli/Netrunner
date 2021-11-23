using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WampSharp.V2.MetaApi;

namespace Netrunner.Server.Services.Internal.Authorization
{
    public class ChatRoomAuthorizer : AuthorizationRule
    {
        private readonly IUserManager _userManager;

        public ChatRoomAuthorizer(string pattern, IUserManager userManager) : base(pattern)
        {
            _userManager = userManager;
        }

        protected override async Task<bool> AuthorizeAsync(Match match, WampSessionDetails session)
        {
            var roomId = match.Groups["roomId"].Value;
            var user = await _userManager.GetUserAsync(session.AuthId);
            if (user == null)
                return false;
            if (user.Rooms != null && user.Rooms.Contains(roomId))
                return true;

            return false;
        }
    }
}