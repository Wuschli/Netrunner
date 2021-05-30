using System.Threading.Tasks;
using WampSharp.V2.Rpc;

namespace Netrunner.Shared.Services
{
    public interface IPingService
    {
        [WampProcedure("netrunner.ping")]
        Task<string> Ping();
    }
}