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

    protected HttpResponseException NotFoundException()
    {
        return new HttpResponseException(StatusCodes.Status404NotFound);
    }

    protected HttpResponseException UnauthorizedException()
    {
        return new HttpResponseException(StatusCodes.Status401Unauthorized);
    }

    protected HttpResponseException BadRequestException(string details)
    {
        return new HttpResponseException(StatusCodes.Status400BadRequest, details);
    }

    protected HttpResponseException InternalServerError(string? details = null)
    {
        return new HttpResponseException(StatusCodes.Status500InternalServerError, details);
    }
}