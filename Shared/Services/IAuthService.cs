using System.Threading.Tasks;
using Netrunner.Shared.Auth;
using WampSharp.V2.Rpc;

namespace Netrunner.Shared.Services
{
    public interface IAuthService
    {
        [WampProcedure("netrunner.auth.register")]
        Task<AuthenticationResponse> Register(RegistrationRequest request);

        [WampProcedure("netrunner.auth.login")]
        Task<AuthenticationResponse> Login(LoginRequest request);
    }

    public interface IAuthenticator
    {
        [WampProcedure("netrunner.auth.authenticate")]
        Task<AuthenticationResult> Authenticate(string realm, string authId, AuthenticationDetails details);
    }

    public class AuthenticationResult
    {
        public string Realm { get; set; }
        public string Role { get; set; }
        public AuthenticationExtras Extra { get; set; }
    }

    public class AuthenticationExtras
    {
    }

    public class AuthenticationDetails
    {
        public long Session { get; set; }
        public string Ticket { get; set; }
    }
}