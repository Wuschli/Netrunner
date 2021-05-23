using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Netrunner.Server.Configs;
using Netrunner.Server.Services;
using Netrunner.Server.Services.Internal;
using Netrunner.Shared.Resources;

namespace Netrunner.Server.Controllers.V1.Resources
{
    public class ResourcesController
    {
        private readonly IUserManager _userManager;
        private readonly IUserManager _userService;
        private readonly IMongoCollection<Resource> _resources;


        public ResourcesController(NetrunnerConfig config, IUserManager userManager, IUserManager userService)
        {
            _userManager = userManager;
            _userService = userService;
            var mongoClient = new MongoClient(config.Database.ConnectionString);
            var database = mongoClient.GetDatabase(config.Database.DatabaseName);
            _resources = database.GetCollection<Resource>(config.Database.ChatRoomCollectionName);
        }

        public async Task<Resource> GetResource(string id)
        {
            throw new NotImplementedException();
            //var user = await _userService.GetCurrentUser();
            //if (user == null)
            //    return Forbid();
            //return Ok();
        }
    }
}