using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Netrunner.Server.Identity.Data;
using Netrunner.Server.Services;
using Netrunner.Shared.Chat;
using Netrunner.Shared.User;

namespace Netrunner.Server.Controllers.V1.User
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userService = userService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetAllUsers()
        {
            return Ok(_userManager.Users.AsEnumerable().Select(_mapper.Map<UserProfile>));
        }
    }
}