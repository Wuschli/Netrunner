using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WampSharp.V2.MetaApi;

namespace Netrunner.ServerLegacy.Services.Internal.Authorization
{
    public abstract class AuthorizationRule
    {
        private readonly Regex _regex;

        protected AuthorizationRule(string pattern)
        {
            _regex = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        public async Task<bool> MatchAsync(string uri, WampSessionDetails session)
        {
            var match = _regex.Match(uri);
            if (!match.Success)
                return true;
            return await AuthorizeAsync(match, session);
        }

        protected abstract Task<bool> AuthorizeAsync(Match match, WampSessionDetails session);
    }
}