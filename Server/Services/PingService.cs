using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Netrunner.Shared.Services;

namespace Netrunner.Server.Services
{
    public class PingService : WampServiceBase, IPingService
    {
        private readonly ILogger<PingService> _logger;

        public PingService(ILogger<PingService> logger)
        {
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            _logger.LogInformation("PingService started");
        }

        public Task<string> Ping()
        {
            return Task.FromResult("Pong");
        }
    }
}