using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Netrunner.Server.Identity.Data;

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

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
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

        public Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            throw new NotImplementedException();
            //return _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }
    }

    public interface IUserManager
    {
        IQueryable<ApplicationUser> Users { get; }

        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<ApplicationUser?> GetCurrentUser();
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task SignInAsync(ApplicationUser user, bool isPersistent, string? authenticationMethod = null);
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
    }
}