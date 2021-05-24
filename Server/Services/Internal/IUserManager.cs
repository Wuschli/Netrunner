using System.Threading.Tasks;
using Netrunner.Shared.Internal;
using Netrunner.Shared.Internal.Auth;

namespace Netrunner.Server.Services.Internal
{
    public interface IUserManager
    {
        Task<ApplicationUser?> GetCurrentUserAsync();
        Task<OperationResult> UpdateAsync(ApplicationUser user);
        Task<ApplicationUser?> FindByNameAsync(string username);
        Task<ApplicationUser?> GetUserAsync(string userId);
    }
}