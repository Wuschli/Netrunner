using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Netrunner.ServerLegacy.Attributes;
using Netrunner.ServerLegacy.Configs;
using Netrunner.ServerLegacy.Services.Internal;
using Netrunner.Shared;
using Netrunner.Shared.Chat;
using Netrunner.Shared.Internal.Auth;
using WampSharp.V2.Core.Contracts;

namespace Netrunner.ServerLegacy.Services
{
    [WampService]
    public class ChatService
    {
        private readonly IMongoCollection<ChatRoom> _rooms;
        private readonly IUserManager _userManager;
        private readonly IWampClientService _wampClientService;
        private readonly IMongoCollection<ChatMessage> _messages;

        public ChatService(NetrunnerConfig config, IUserManager userManager, IWampClientService wampClientService)
        {
            _userManager = userManager;
            _wampClientService = wampClientService;
            var mongoClient = new MongoClient(config.Database.ConnectionString);
            var database = mongoClient.GetDatabase(config.Database.DatabaseName);
            _rooms = database.GetCollection<ChatRoom>(config.Database.ChatRoomCollectionName);
            _messages = database.GetCollection<ChatMessage>(config.Database.ChatMessageCollectionName);
        }

        public async Task CreateRoom(CreateChatRoom room)
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                throw new WampException("netrunner.error.not_authorized");
            var dbRoom = new ChatRoom
            {
                Name = room.Name,
                Invitations = new List<Guid>(),
                Members = new List<Guid> { user.Id }
            };
            await _rooms.InsertOneAsync(dbRoom);

            var identityResult = await AddRoomToUser(user, dbRoom.Id);
            if (!identityResult.Succeeded)
                throw new WampException("netrunner.error.operation_failed");
        }

        public async Task<List<Guid>?> GetInvites()
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                throw new WampException("netrunner.error.not_authorized");
            if (user.Invitations == null || !user.Invitations.Any())
                return new List<Guid>();
            return user.Invitations;
        }

        public async Task<List<ChatMessage>?> GetMessages(Guid roomId, int? skip = null)
        {
            if (roomId == Guid.Empty)
                throw new WampException("netrunner.error.bad_request");

            // TODO check user authorization for room
            var result = await _messages
                .Find(message => message.RoomId == roomId)
                .SortByDescending(message => message.Timestamp)
                .Limit(50)
                .Skip(skip)
                .ToListAsync();
            return result;
        }

        public async Task<ChatRoom?> GetRoomDetails(Guid roomId)
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                return null;
            var room = await _rooms.Find(r => r.Id == roomId).FirstOrDefaultAsync();
            if (room == null)
                return null;
            if (room.Members.Contains(user.Id) || room.Invitations.Contains(user.Id))
                return room;
            return null;
        }

        public async Task<List<ChatRoom>?> GetRooms()
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                return null;

            if (user.Rooms == null || !user.Rooms.Any())
                return new List<ChatRoom>();

            var filter = Builders<ChatRoom>.Filter.In(room => room.Id, user.Rooms);
            return await _rooms.Find(filter).ToListAsync();
        }

        public async Task InviteUserToRoom(Guid roomId, string username)
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                throw new WampException("netrunner.error.not_authorized");

            var invited = await _userManager.FindByNameAsync(username);
            if (invited == null)
                throw new WampException("netrunner.error.not_found");

            var room = await _rooms.Find(r => r.Id == roomId).FirstAsync();
            if (room == null)
                throw new WampException("netrunner.error.not_found");

            if (!room.Members.Contains(user.Id))
                throw new WampException("netrunner.error.not_found");

            if (room.Invitations?.Contains(invited.Id) == true)
                return;

            var update = Builders<ChatRoom>.Update
                .Push(r => r.Invitations, invited.Id);

            var result = await _rooms.FindOneAndUpdateAsync(r => r.Id == roomId, update);
            if (result == null)
                throw new WampException("netrunner.error.not_found");

            var identityResult = await AddInviteToUser(invited, room.Id);
            if (!identityResult.Succeeded)
                throw new WampException("netrunner.error.operation_failed");
        }

        public async Task JoinRoom(Guid roomId)
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                throw new WampException("netrunner.error.not_authorized");

            var room = await _rooms.Find(r => r.Id == roomId).FirstAsync();
            if (room == null)
                throw new WampException("netrunner.error.not_found");

            if (room.Members.Contains(user.Id))
                return;

            if (room.Invitations == null || !room.Invitations.Contains(user.Id))
                throw new WampException("netrunner.error.not_authorized");

            var update = Builders<ChatRoom>.Update
                .Push(r => r.Members, user.Id)
                .Pull(r => r.Invitations, user.Id);

            var result = await _rooms.FindOneAndUpdateAsync(r => r.Id == roomId, update);
            if (result == null)
                throw new WampException("netrunner.error.not_found");

            var identityResult = await AddRoomToUser(user, room.Id);
            if (!identityResult.Succeeded)
                throw new WampException("netrunner.error.operation_failed");
        }

        public async Task LeaveRoom(Guid roomId)
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                throw new WampException("netrunner.error.not_authorized");

            var room = await _rooms.Find(r => r.Id == roomId).FirstAsync();
            if (room == null)
                throw new WampException("netrunner.error.not_found");

            if (!room.Members.Contains(user.Id))
                throw new WampException("netrunner.error.not_found");

            var update = Builders<ChatRoom>.Update
                .Pull(r => r.Members, user.Id);

            var options = new FindOneAndUpdateOptions<ChatRoom>
            {
                ReturnDocument = ReturnDocument.After
            };

            var result = await _rooms.FindOneAndUpdateAsync<ChatRoom>(r => r.Id == roomId, update, options);
            if (result == null)
                throw new WampException("netrunner.error.not_found");

            if (!result.Members.Any())
            {
                var deleteResult = await DeleteRoom(result.Id);
            }

            var identityResult = await RemoveRoomFromUser(user, room.Id);
            if (!identityResult.Succeeded)
                throw new WampException("netrunner.error.operation_failed");
        }

        public async Task SendMessage(ChatMessage message)
        {
            var user = await _userManager.GetCurrentUserAsync();
            if (user == null)
                throw new WampException("netrunner.error.not_authorized");

            if (message.RoomId == Guid.Empty)
                throw new WampException("netrunner.error.bad_request");

            var room = await _rooms.Find(r => r.Id == message.RoomId).FirstOrDefaultAsync();
            if (room == null)
                throw new WampException("netrunner.error.not_found");

            if (room.Members == null || !room.Members.Contains(user.Id))
                throw new WampException("netrunner.error.not_authorized");

            var dbMessage = new ChatMessage
            {
                SenderId = user.Id,
                Timestamp = DateTime.Now,
                Message = message.Message,
                RoomId = message.RoomId
            };

            await _messages.InsertOneAsync(dbMessage);

            await _wampClientService.PublishAsync<ChatMessage>($"netrunner.chat.{message.RoomId}.messages", dbMessage);
        }


        private async Task<OperationResult> AddInviteToUser(ApplicationUser user, Guid roomId)
        {
            if (user.Invitations == null!)
                user.Invitations = new List<Guid>();

            user.Invitations.Add(roomId);

            return await _userManager.UpdateAsync(user);
        }

        private async Task<OperationResult> AddRoomToUser(ApplicationUser user, Guid roomId)
        {
            if (user.Rooms == null!)
                user.Rooms = new List<Guid>();
            if (user.Invitations == null!)
                user.Invitations = new List<Guid>();

            user.Rooms.Add(roomId);
            user.Invitations.Remove(roomId);

            return await _userManager.UpdateAsync(user);
        }

        private async Task<OperationResult> RemoveRoomFromUser(ApplicationUser user, Guid roomId)
        {
            if (user.Rooms == null!)
                user.Rooms = new List<Guid>();
            if (user.Invitations == null!)
                user.Invitations = new List<Guid>();

            user.Rooms.Remove(roomId);

            return await _userManager.UpdateAsync(user);
        }

        private async Task<DeleteResult> DeleteRoom(Guid roomId)
        {
            return await _rooms.DeleteOneAsync(room => room.Id == roomId);
        }
    }
}