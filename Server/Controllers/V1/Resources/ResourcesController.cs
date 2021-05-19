using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Netrunner.Server.Configs;
using Netrunner.Server.Identity.Data;
using Netrunner.Server.Services;
using Netrunner.Shared.Chat;
using Netrunner.Shared.Resources;

namespace Netrunner.Server.Controllers.V1.Resources
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class ResourcesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserManager _userService;
        private readonly IMongoCollection<Resource> _resources;


        public ResourcesController(NetrunnerConfig config, UserManager<ApplicationUser> userManager, IUserManager userService)
        {
            _userManager = userManager;
            _userService = userService;
            var mongoClient = new MongoClient(config.Database.ConnectionString);
            var database = mongoClient.GetDatabase(config.Database.DatabaseName);
            _resources = database.GetCollection<Resource>(config.Database.ChatRoomCollectionName);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Resource>> GetResource(string id)
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return Forbid();
            return Ok();
        }
    }
}