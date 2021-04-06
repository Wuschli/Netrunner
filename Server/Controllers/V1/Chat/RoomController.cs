using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using Netrunner.Server.Identity.Data;
using Netrunner.Server.Models;
using Netrunner.Server.Services;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Controllers.V1.Chat
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IMongoCollection<ChatRoom> _rooms;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public RoomController(IDatabaseSettings settings, UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _rooms = database.GetCollection<ChatRoom>(settings.ChatRoomCollectionName);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatRoom>>> GetAllRooms()
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return Forbid();

            var filter = Builders<ChatRoom>.Filter.In(room => room.Id, user.Rooms);
            return await _rooms.Find(filter).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatRoom>> GetRoom(string id)
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return Forbid();
            var room = await _rooms.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (room == null)
                return NotFound();
            if (room.Members.Contains(user.Id) || room.Invitations.Contains(user.Id))
                return room;
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateChatRoom room)
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return Forbid();
            var dbRoom = new ChatRoom
            {
                Name = room.Name,
                Invitations = new List<Guid>(),
                Members = new List<Guid> {user.Id}
            };
            await _rooms.InsertOneAsync(dbRoom);

            var identityResult = await AddRoomToUser(user, dbRoom.Id);
            if (identityResult.Succeeded)
                return CreatedAtAction(nameof(GetRoom), new {id = dbRoom.Id}, dbRoom);
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not add room to user");
        }

        [HttpPost("join/{id}")]
        public async Task<ActionResult> Join(string id)
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return Forbid();

            var room = await _rooms.Find(r => r.Id == id).FirstAsync();
            if (room == null)
                return NotFound();

            if (room.Members.Contains(user.Id))
                return Ok();

            if (room.Invitations == null || !room.Invitations.Contains(user.Id))
                return Forbid();

            var update = Builders<ChatRoom>.Update
                .Push(r => r.Members, user.Id)
                .Pull(r => r.Invitations, user.Id);

            var result = await _rooms.FindOneAndUpdateAsync(r => r.Id == id, update);
            if (result == null)
                return NotFound();

            var identityResult = await AddRoomToUser(user, room.Id);
            if (identityResult.Succeeded)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError, "Could not add room to user");
        }

        private async Task<IdentityResult> AddRoomToUser(ApplicationUser user, string roomId)
        {
            if (user.Rooms == null!)
                user.Rooms = new List<string>();
            if (user.Invitations == null!)
                user.Invitations = new List<string>();

            user.Rooms.Add(roomId);
            user.Invitations.Remove(roomId);

            IdentityResult result = IdentityResult.Failed();
            for (int i = 0; i < 5; i++)
            {
                result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return result;
            }

            return result;
        }
    }
}