using System.Threading.Tasks;
using Netrunner.Shared.Auth;
using WampSharp.V2.Rpc;

namespace Netrunner.Shared.Services
{
    public interface IAuthenticator
    {
        [WampProcedure("netrunner.auth.authenticate")]
        Task<AuthenticationResult> Authenticate(string realm, string authId, AuthenticationDetails details);

        [WampProcedure("netrunner.auth.authenticate_simple")]
        Task<string> AuthenticateSimple(string realm, string authId, AuthenticationDetails details);
    }
}