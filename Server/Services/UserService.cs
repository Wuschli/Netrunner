using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Netrunner.Server.Identity.Data;

namespace Netrunner.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        private HttpContext? Context => _httpContextAccessor.HttpContext;

        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetCurrentUser()
        {
            if (Context?.User == null)
                return null;

            var user = await _userManager.GetUserAsync(Context.User);
            return user;
        }
    }

    public interface IUserService
    {
        Task<ApplicationUser?> GetCurrentUser();
    }
}