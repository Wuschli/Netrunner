using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Netrunner.Shared.Services;

namespace Netrunner.Server.Services
{
    public class ChallengeService : WampServiceBase, IChallengeService
    {
        private readonly ILogger<ChallengeService> _logger;

        public ChallengeService(ILogger<ChallengeService> logger)
        {
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            _logger.LogInformation("ChallengeService started");
        }

        public Task<string> Ping()
        {
            return Task.FromResult("Pong");
        }
    }
}