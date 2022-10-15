using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Netrunner.Server.Helpers;
using Netrunner.Server.Services;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ChatController : NetrunnerController
{
    private readonly IMongoCollection<ChatMessage> _messages;

    public ChatController(IUsersService users, IConfiguration config) : base(users)
    {
        var mongoClient = new MongoClient(config["DB:ConnectionString"]);
        var database = mongoClient.GetDatabase(config["DB:DatabaseName"]);
        _messages = database.GetCollection<ChatMessage>(Constants.MessagesCollection);
    }

    [HttpGet("{roomId:guid}")]
    public async Task<List<ChatMessage>?> GetMessages(Guid roomId, int? skip = null)
    {
        if (roomId == Guid.Empty)
            throw BadRequestException("roomId must not be null");

        var user = await GetUser();
        if (!user.Rooms.Contains(roomId))
            throw UnauthorizedException();

        var result = await _messages
            .Find(message => message.RoomId == roomId)
            .SortByDescending(message => message.Timestamp)
            .Limit(50)
            .Skip(skip)
            .ToListAsync();
        return result;
    }

    [HttpPost("{roomId:guid}")]
    public async Task SendMessage(Guid roomId, SendChatMessage message)
    {
        var user = await GetUser();

        if (roomId == Guid.Empty)
            throw BadRequestException("roomId must not be null");

        if (!user.Rooms.Contains(roomId))
            throw UnauthorizedException();

        var dbMessage = new ChatMessage
        {
            SenderId = user.Id,
            Timestamp = DateTime.Now,
            Message = message.Message,
            RoomId = roomId
        };

        await _messages.InsertOneAsync(dbMessage);
        //TODO publish
    }
}