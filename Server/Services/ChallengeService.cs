using System.Threading.Tasks;
using Netrunner.Shared.Services;

namespace Netrunner.Server.Services
{
    public class ChallengeService : IChallengeService
    {
        public Task<string> Ping()
        {
            return Task.FromResult("Pong");
        }
    }
}