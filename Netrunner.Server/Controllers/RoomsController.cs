using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Netrunner.Server.Helpers;
using Netrunner.Server.Services;
using Netrunner.Shared;
using Netrunner.Shared.Chat;
using Netrunner.Shared.Users;

namespace Netrunner.Server.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class RoomsController : NetrunnerController
{
    private readonly IMongoCollection<ChatRoom> _rooms;

    public RoomsController(IConfiguration config, IUsersService users) : base(users)
    {
        var mongoClient = new MongoClient(config["DB:ConnectionString"]);
        var database = mongoClient.GetDatabase(config["DB:DatabaseName"]);
        _rooms = database.GetCollection<ChatRoom>(Constants.RoomCollection);
    }

    [HttpGet]
    public async Task<IEnumerable<ChatRoom>> Get()
    {
        var user = await GetUser();

        if (!user.Rooms.Any())
            return Enumerable.Empty<ChatRoom>();

        var filter = Builders<ChatRoom>.Filter.In(room => room.Id, user.Rooms);
        return await _rooms.Find(filter).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<ChatRoom>> CreateRoom(CreateChatRoom room)
    {
        var user = await GetUser();
        var dbRoom = new ChatRoom
        {
            Name = room.Name,
            Invitations = new List<Guid>(),
            Members = new List<Guid> { user.Id }
        };
        await _rooms.InsertOneAsync(dbRoom);

        await AddRoomToUser(user, dbRoom.Id);
        return Ok(dbRoom);
    }


    private async Task<OperationResult> AddRoomToUser(ApplicationUser user, Guid roomId)
    {
        user.Rooms.Add(roomId);
        user.Invitations.Remove(roomId);

        return await Users.UpdateAsync(user);
    }
}