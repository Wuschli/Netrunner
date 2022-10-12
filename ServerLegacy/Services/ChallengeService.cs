using System.Threading.Tasks;
using Netrunner.Shared.Services;

namespace Netrunner.ServerLegacy.Services
{
    public class ChallengeService : IChallengeService
    {
        public Task<string> Ping()
        {
            return Task.FromResult("Pong");
        }
    }
}