using System.Threading.Tasks;
using Netrunner.Server.Attributes;
using Netrunner.Shared.Services;

namespace Netrunner.Server.Services
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