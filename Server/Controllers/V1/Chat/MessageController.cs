using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using Netrunner.Server.Hubs;
using Netrunner.Server.Models;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Controllers.V1.Chat
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IHubContext<ChatHub, IChatHub> _chatHubContext;
        private readonly IMongoCollection<ChatRoom> _rooms;
        private readonly IMongoCollection<ChatMessage> _messages;

        public MessageController(IDatabaseSettings settings, IHubContext<ChatHub, IChatHub> chatHubContext)
        {
            _chatHubContext = chatHubContext;
            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _rooms = database.GetCollection<ChatRoom>(settings.ChatRoomCollectionName);
            _messages = database.GetCollection<ChatMessage>(settings.ChatMessageCollectionName);
        }


        [HttpGet("{roomId}")]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetMessages(string roomId, int? skip)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return BadRequest("No room given");

            var result = await _messages
                .Find(message => message.RoomId == roomId)
                .SortByDescending(message => message.Timestamp)
                .Limit(50)
                .Skip(skip)
                .ToListAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage([FromBody] ChatMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.RoomId))
                return BadRequest("No room given");

            message.Timestamp = DateTime.Now;

            var room = await _rooms.Find(r => r.Id == message.RoomId).FirstOrDefaultAsync();
            if (room == null)
                return NotFound("Room not found");

            await _messages.InsertOneAsync(message);

            await _chatHubContext.Clients.All.ReceiveMessage(message);

            return Ok();
        }
    }
}