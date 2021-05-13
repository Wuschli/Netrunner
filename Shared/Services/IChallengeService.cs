using System.Threading.Tasks;

namespace Netrunner.Shared.Services
{
    public interface IChallengeService
    {
        Task<string> Ping();
    }
}