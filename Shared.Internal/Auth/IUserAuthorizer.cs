using System.Collections.Generic;
using System.Threading.Tasks;
using WampSharp.V2.MetaApi;
using WampSharp.V2.Rpc;

namespace Netrunner.Shared.Internal.Auth
{
    public interface IUserAuthorizer
    {
        [WampProcedure("netrunner.auth.authorize_user")]
        Task<AuthorizationResult> AuthorizeUser(WampSessionDetails session, string uri, string action, Dictionary<string, object> options);
    }
}