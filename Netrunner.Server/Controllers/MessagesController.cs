using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MQTTnet;
using MQTTnet.Server;
using Netrunner.Server.Helpers;
using Netrunner.Server.Services;
using Netrunner.Shared.Chat;
using Newtonsoft.Json;

namespace Netrunner.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MessagesController : NetrunnerController
{
    private readonly MqttServer _mqttServer;
    private readonly IMongoCollection<ChatMessage> _messages;

    public MessagesController(IUsersService users, IConfiguration config, MqttServer mqttServer) : base(users)
    {
        _mqttServer = mqttServer;
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

        var mqttMessage = new MqttApplicationMessageBuilder().WithTopic(roomId.ToString()).WithPayload(dbMessage.Id.ToString()).Build();
        await _mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(mqttMessage)
        {
            SenderClientId = "Server"
        });
    }
}