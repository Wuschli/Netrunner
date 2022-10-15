using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netrunner.Server.Services;
using Netrunner.Shared.Users;

namespace Netrunner.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : NetrunnerController
{
    public UsersController(IUsersService users) : base(users)
    {
    }

    [HttpGet("me")]
    public async Task<ActionResult<ApplicationUser>> GetMe()
    {
        var user = await GetUser();
        return Ok(user);
    }
}