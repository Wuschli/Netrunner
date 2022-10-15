using AutoMapper;
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
    private readonly IMapper _mapper;

    public UsersController(IUsersService users, IMapper mapper) : base(users)
    {
        _mapper = mapper;
    }

    [HttpGet("me")]
    public async Task<ApplicationUser> GetMe()
    {
        var user = await GetUser();
        return user;
    }

    [HttpGet("{userId:guid}")]
    public async Task<UserProfile> GetUser(Guid userId)
    {
        var user = await Users.GetUser(userId);
        if (user == null)
            throw NotFoundException();
        return _mapper.Map<UserProfile>(user);
    }
}