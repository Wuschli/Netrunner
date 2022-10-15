using System.Threading.Tasks;
using Netrunner.Shared;
using Netrunner.Shared.Internal.Auth;

namespace Netrunner.ServerLegacy.Services.Internal
{
    public interface IUserManager
    {
        Task<ApplicationUser?> GetCurrentUserAsync();
        Task<OperationResult> UpdateAsync(ApplicationUser user);
        Task<ApplicationUser?> FindByNameAsync(string username);
        Task<ApplicationUser?> GetUserAsync(string userId);
    }
}