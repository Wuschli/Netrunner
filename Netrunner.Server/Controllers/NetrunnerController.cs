using Microsoft.AspNetCore.Mvc;
using Netrunner.Server.Helpers;
using Netrunner.Server.Services;
using Netrunner.Shared.Users;

namespace Netrunner.Server.Controllers;

public abstract class NetrunnerController : ControllerBase
{
    public IUsersService Users { get; }

    protected NetrunnerController(IUsersService users)
    {
        Users = users;
    }

    protected async Task<ApplicationUser> GetUser()
    {
        return await Users.GetOrCreateUser(User.GetId());
    }
}