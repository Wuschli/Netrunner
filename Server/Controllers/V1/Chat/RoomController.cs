using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using Netrunner.Server.Identity.Data;
using Netrunner.Server.Models;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Controllers.V1.Chat
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IMongoCollection<ChatRoom> _rooms;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoomController(IDatabaseSettings settings, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _rooms = database.GetCollection<ChatRoom>(settings.ChatRoomCollectionName);
        }

        [HttpGet]
        public async Task<IEnumerable<ChatRoom>> Get()
        {
            return await _rooms.Find(FilterDefinition<ChatRoom>.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatRoom>> Get(string id)
        {
            return await _rooms.Find(room => room.Id == id).FirstAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ChatRoom value)
        {
            await _rooms.InsertOneAsync(value);
            return Ok();
        }

        [HttpPost("join/{id}")]
        public async Task<ActionResult> Join(string id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId))
                return Forbid();

            var room = await _rooms.Find(r => r.Id == id).FirstAsync();
            if (room == null)
                return NotFound();

            if (room.Members.Contains(userId))
                return Ok();

            if (room.Invitations == null || !room.Invitations.Contains(userId))
                return Forbid();

            var update = Builders<ChatRoom>.Update
                .Push(r => r.Members, userId)
                .Pull(r => r.Invitations, userId);

            var result = await _rooms.FindOneAndUpdateAsync(r => r.Id == id, update);
            if (result == null)
                return NotFound();

            var user = _userManager.Users.Single(r => r.Id == userId);

            if (user.Rooms == null!)
                user.Rooms = new List<string>();
            if (user.Invitations == null!)
                user.Invitations = new List<string>();


            user.Rooms.Add(room.Id);
            user.Invitations.Remove(room.Id);
            for (int i = 0; i < 5; i++)
            {
                var identityResult = await _userManager.UpdateAsync(user);
                if (identityResult.Succeeded)
                    return Ok();
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Could not add room to user");
        }
    }
}