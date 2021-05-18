using System.Threading.Tasks;
using Netrunner.Shared.Identity;
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
}