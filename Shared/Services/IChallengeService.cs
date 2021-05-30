using System.Threading.Tasks;
using WampSharp.V2.Rpc;

namespace Netrunner.Shared.Services
{
    public interface IChallengeService
    {
        [WampProcedure("netrunner.challenges.ping")]
        Task<string> Ping();
    }
}