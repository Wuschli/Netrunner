using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Netrunner.Server.Models;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Controllers.V1.Chat
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMongoCollection<ChatRoom> _rooms;

        public MessageController(IDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _rooms = database.GetCollection<ChatRoom>(settings.ChatCollectionName);
        }

        [HttpPost("{roomId}")]
        public async Task<ActionResult> SendMessage(string roomId, [FromBody] ChatMessage message)
        {
            message.Timestamp = DateTime.Now;
            var update = Builders<ChatRoom>.Update
                .Push(room => room.Messages, message);
            var result = await _rooms.FindOneAndUpdateAsync(room => room.Id == roomId, update);
            return Ok();
        }
    }
}