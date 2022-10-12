using System.Threading.Tasks;
using Netrunner.ServerLegacy.Attributes;
using Netrunner.Shared.Services;

namespace Netrunner.ServerLegacy.Services
{
    [WampService]
    public class PingService : IPingService
    {
        public Task<string> Ping()
        {
            return Task.FromResult("Pong");
        }
    }
}