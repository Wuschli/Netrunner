using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WampSharp.V2.MetaApi;

namespace Netrunner.Server.Services.Internal.Authorization
{
    public class RoleAuthorizer : AuthorizationRule
    {
        private readonly string _role;
        private readonly IUserManager _userManager;

        public RoleAuthorizer(string pattern, string role, IUserManager userManager) : base(pattern)
        {
            _role = role;
            _userManager = userManager;
        }

        protected override async Task<bool> AuthorizeAsync(Match match, WampSessionDetails session)
        {
            var user = await _userManager.GetUserAsync(session.AuthId);
            if (user == null)
                return false;
            if (user.Roles.Contains(_role))
                return true;

            return false;
        }
    }
}