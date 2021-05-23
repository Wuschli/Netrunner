using System.Threading.Tasks;
using Netrunner.Shared.Auth;
using WampSharp.V2.Rpc;

namespace Netrunner.Shared.Internal.Auth
{
    public interface IAuthenticator
    {
        [WampProcedure("netrunner.auth.authenticate")]
        Task<AuthenticationResult> Authenticate(string realm, string authId, AuthenticationDetails details);
    }
}