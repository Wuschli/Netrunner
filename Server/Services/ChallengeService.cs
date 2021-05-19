using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Netrunner.Shared.Services;

namespace Netrunner.Server.Services
{
    public class ChallengeService : IChallengeService
    {
        private readonly ILogger<ChallengeService> _logger;

        public ChallengeService(ILogger<ChallengeService> logger)
        {
            _logger = logger;
        }

        public Task<string> Ping()
        {
            return Task.FromResult("Pong");
        }
    }
}