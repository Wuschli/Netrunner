using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Netrunner.Server.Configs;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Controllers.V1.Chat
{
    public class MessageController
    {
        private readonly IMongoCollection<ChatRoom> _rooms;
        private readonly IMongoCollection<ChatMessage> _messages;

        public MessageController(NetrunnerConfig config)
        {
            var mongoClient = new MongoClient(config.Database.ConnectionString);
            var database = mongoClient.GetDatabase(config.Database.DatabaseName);
            _rooms = database.GetCollection<ChatRoom>(config.Database.ChatRoomCollectionName);
            _messages = database.GetCollection<ChatMessage>(config.Database.ChatMessageCollectionName);
        }


        public async Task<IEnumerable<ChatMessage>> GetMessages(string roomId, int? skip)
        {
            throw new NotImplementedException();
            //if (string.IsNullOrWhiteSpace(roomId))
            //    return BadRequest("No room given");

            //var result = await _messages
            //    .Find(message => message.RoomId == roomId)
            //    .SortByDescending(message => message.Timestamp)
            //    .Limit(50)
            //    .Skip(skip)
            //    .ToListAsync();
            //return Ok(result);
        }

        public async Task SendMessage(SendChatMessage message)
        {
            throw new NotImplementedException();
            //if (string.IsNullOrWhiteSpace(message.RoomId))
            //    return BadRequest("No room given");

            //var room = await _rooms.Find(r => r.Id == message.RoomId).FirstOrDefaultAsync();
            //if (room == null)
            //    return NotFound("Room not found");

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (room.Members == null || !room.Members.Contains(userId))
            //    return Forbid();

            //var dbMessage = new ChatMessage
            //{
            //    SenderId = userId,
            //    Timestamp = DateTime.Now,
            //    Message = message.Message,
            //    RoomId = message.RoomId
            //};

            //await _messages.InsertOneAsync(dbMessage);

            //await _chatHubContext.Clients.Group($"room{dbMessage.RoomId}").ReceiveMessage(dbMessage);

            //return Ok();
        }
    }
}