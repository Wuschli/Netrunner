using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Netrunner.Server.Configs;
using Netrunner.Server.Identity.Data;
using Netrunner.Server.Services;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Controllers.V1.Chat
{
    public class RoomsController // : ControllerBase
    {
        private readonly IMongoCollection<ChatRoom> _rooms;
        private readonly IUserManager _userManager;
        private readonly IUserManager _userService;

        public RoomsController(NetrunnerConfig config, IUserManager userManager,
            IUserManager userService)
        {
            _userManager = userManager;
            _userService = userService;
            var mongoClient = new MongoClient(config.Database.ConnectionString);
            var database = mongoClient.GetDatabase(config.Database.DatabaseName);
            _rooms = database.GetCollection<ChatRoom>(config.Database.ChatRoomCollectionName);
        }

        public async Task<IEnumerable<ChatRoom>> GetAllRooms()
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return null;

            if (user.Rooms == null || !user.Rooms.Any())
                return new List<ChatRoom>();

            var filter = Builders<ChatRoom>.Filter.In(room => room.Id, user.Rooms);
            return await _rooms.Find(filter).ToListAsync();
        }

        public async Task<ChatRoom> GetRoom(string id)
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return null;
            var room = await _rooms.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (room == null)
                return null;
            if (room.Members.Contains(user.Id) || room.Invitations.Contains(user.Id))
                return room;
            return null;
        }

        public async Task<bool> Create(CreateChatRoom room)
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
                return false;
            var dbRoom = new ChatRoom
            {
                Name = room.Name,
                Invitations = new List<string>(),
                Members = new List<string> {user.Id}
            };
            await _rooms.InsertOneAsync(dbRoom);

            var identityResult = await AddRoomToUser(user, dbRoom.Id);
            throw new NotImplementedException();
            //if (identityResult.Succeeded)
            //    return CreatedAtAction(nameof(GetRoom), new {id = dbRoom.Id}, dbRoom);
            //return StatusCode(StatusCodes.Status500InternalServerError, "Could not add room to user");
        }

        public async Task Join(string id)
        {
            throw new NotImplementedException();
            //var user = await _userService.GetCurrentUser();
            //if (user == null)
            //    return Forbid();

            //var room = await _rooms.Find(r => r.Id == id).FirstAsync();
            //if (room == null)
            //    return NotFound();

            //if (room.Members.Contains(user.Id))
            //    return Ok();

            //if (room.Invitations == null || !room.Invitations.Contains(user.Id))
            //    return Forbid();

            //var update = Builders<ChatRoom>.Update
            //    .Push(r => r.Members, user.Id)
            //    .Pull(r => r.Invitations, user.Id);

            //var result = await _rooms.FindOneAndUpdateAsync(r => r.Id == id, update);
            //if (result == null)
            //    return NotFound();

            //var identityResult = await AddRoomToUser(user, room.Id);
            //if (identityResult.Succeeded)
            //    return Ok();

            //return StatusCode(StatusCodes.Status500InternalServerError, "Could not add room to user");
        }

        public async Task Leave(string id)
        {
            throw new NotImplementedException();
            //var user = await _userService.GetCurrentUser();
            //if (user == null)
            //    return Forbid();

            //var room = await _rooms.Find(r => r.Id == id).FirstAsync();
            //if (room == null)
            //    return NotFound();

            //if (!room.Members.Contains(user.Id))
            //    return NotFound();

            //var update = Builders<ChatRoom>.Update
            //    .Pull(r => r.Members, user.Id);

            //var options = new FindOneAndUpdateOptions<ChatRoom>
            //{
            //    ReturnDocument = ReturnDocument.After
            //};

            //var result = await _rooms.FindOneAndUpdateAsync<ChatRoom>(r => r.Id == id, update, options);
            //if (result == null)
            //    return NotFound();

            //if (!result.Members.Any())
            //{
            //    var deleteResult = await DeleteRoom(result.Id);
            //}

            //var identityResult = await RemoveRoomFromUser(user, room.Id);
            //if (identityResult.Succeeded)
            //    return Ok();

            //return StatusCode(StatusCodes.Status500InternalServerError, "Could not remove room from user");
        }

        public async Task<IEnumerable<string>> GetInvites()
        {
            throw new NotImplementedException();
            //var user = await _userService.GetCurrentUser();
            //if (user == null)
            //    return Forbid();
            //if (user.Invitations == null || !user.Invitations.Any())
            //    return Ok(new List<string>());
            //return Ok(user.Invitations);
        }

        public async Task Invite(string roomId, string userName)
        {
            throw new NotImplementedException();
            //var user = await _userService.GetCurrentUser();
            //if (user == null)
            //    return Forbid();

            //var invited = await _userManager.FindByNameAsync(userName);
            //if (invited == null)
            //    return NotFound();

            //var room = await _rooms.Find(r => r.Id == roomId).FirstAsync();
            //if (room == null)
            //    return NotFound();

            //if (!room.Members.Contains(user.Id))
            //    return NotFound();

            //if (room.Invitations?.Contains(invited.Id) == true)
            //    return Ok();

            //var update = Builders<ChatRoom>.Update
            //    .Push(r => r.Invitations, invited.Id);

            //var result = await _rooms.FindOneAndUpdateAsync(r => r.Id == roomId, update);
            //if (result == null)
            //    return NotFound();

            //var identityResult = await AddInviteToUser(invited, room.Id);
            //if (identityResult.Succeeded)
            //    return Ok();

            //return StatusCode(StatusCodes.Status500InternalServerError, "Could not add invite to user");
        }


        private async Task<IdentityResult> AddInviteToUser(ApplicationUser user, string roomId)
        {
            if (user.Invitations == null!)
                user.Invitations = new List<string>();

            user.Invitations.Add(roomId);

            return await _userManager.UpdateAsync(user);
        }

        private async Task<IdentityResult> AddRoomToUser(ApplicationUser user, string roomId)
        {
            if (user.Rooms == null!)
                user.Rooms = new List<string>();
            if (user.Invitations == null!)
                user.Invitations = new List<string>();

            user.Rooms.Add(roomId);
            user.Invitations.Remove(roomId);

            return await _userManager.UpdateAsync(user);
        }

        private async Task<IdentityResult> RemoveRoomFromUser(ApplicationUser user, string roomId)
        {
            if (user.Rooms == null!)
                user.Rooms = new List<string>();
            if (user.Invitations == null!)
                user.Invitations = new List<string>();

            user.Rooms.Remove(roomId);

            return await _userManager.UpdateAsync(user);
        }

        private async Task<DeleteResult> DeleteRoom(string roomId)
        {
            return await _rooms.DeleteOneAsync(room => room.Id == roomId);
        }
    }
}