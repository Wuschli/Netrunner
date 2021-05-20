using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Netrunner.Server.Identity.Data;
using Netrunner.Shared.Internal;

namespace Netrunner.Server.Services
{
    public class UserManager : IUserManager
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;

        //private HttpContext? Context => _httpContextAccessor.HttpContext;
        public IQueryable<ApplicationUser> Users => throw new NotImplementedException(); //_userManager.Users;

        public UserManager( /*IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager*/)
        {
            //_httpContextAccessor = httpContextAccessor;
            //_userManager = userManager;
            //_signInManager = signInManager;
        }

        public async Task<ApplicationUser?> GetCurrentUser()
        {
            throw new NotImplementedException();
            //if (Context?.User == null)
            //    return null;

            //var user = await _userManager.GetUserAsync(Context.User);
            //return user;
        }

        public Task<OperationResult> CreateAsync(ApplicationUser user, string password)
        {
            throw new NotImplementedException();
            //return _userManager.CreateAsync(user, password);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
            //return _userManager.GetRolesAsync(user);
        }

        public Task SignInAsync(ApplicationUser user, bool isPersistent, string? authenticationMethod = null)
        {
            throw new NotImplementedException();
            //return _signInManager.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public Task<OperationResult> PasswordSignInAsync(string username, string password)
        {
            throw new NotImplementedException();
            //return _signInManager.PasswordSignInAsync(username, password);
        }

        public Task<OperationResult> UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }

    public interface IUserManager
    {
        IQueryable<ApplicationUser> Users { get; }

        Task<OperationResult> CreateAsync(ApplicationUser user, string password);
        Task<ApplicationUser?> GetCurrentUser();
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task SignInAsync(ApplicationUser user, bool isPersistent, string? authenticationMethod = null);
        Task<OperationResult> PasswordSignInAsync(string username, string password);
        Task<OperationResult> UpdateAsync(ApplicationUser user);
    }
}