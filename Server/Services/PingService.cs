using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Netrunner.Shared.Services;

namespace Netrunner.Server.Services
{
    [WampService]
    public class PingService : IPingService
    {
        private readonly ILogger<PingService> _logger;

        public PingService(ILogger<PingService> logger)
        {
            _logger = logger;
        }

        public Task<string> Ping()
        {
            return Task.FromResult("Pong");
        }
    }
}