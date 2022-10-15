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
        _rooms = database.GetCollection<ChatRoom>(Constants.RoomsCollection);
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

    [HttpGet("{roomId:guid}")]
    public async Task<ChatRoom?> GetRoomDetails(Guid roomId)
    {
        var user = await GetUser();
        if (!user.Rooms.Contains(roomId))
            throw UnauthorizedException();
        var room = await _rooms.Find(r => r.Id == roomId).FirstOrDefaultAsync();
        if (room == null)
            throw NotFoundException();
        if (room.Members.Contains(user.Id) || room.Invitations.Contains(user.Id))
            return room;
        throw UnauthorizedException();
    }

    [HttpPost]
    public async Task<ChatRoom> CreateRoom(CreateChatRoom create)
    {
        var user = await GetUser();
        var room = new ChatRoom
        {
            Name = create.Name,
            Invitations = new List<Guid>(),
            Members = new List<Guid> { user.Id }
        };
        await _rooms.InsertOneAsync(room);

        await AddRoomToUser(user, room.Id);
        return room;
    }

    [HttpGet("invites")]
    public async Task<List<Guid>?> GetInvites()
    {
        var user = await GetUser();
        return user.Invitations;
    }

    [HttpPost("invites")]
    public async Task InviteUserToRoom(CreateInvite invite)
    {
        var user = await GetUser();
        if (!user.Rooms.Contains(invite.RoomId))
            throw UnauthorizedException();

        var invited = await Users.GetUser(invite.UserId);
        if (invited == null)
            throw BadRequestException($"User {invite.UserId} does not exist");

        var update = Builders<ChatRoom>.Update
            .AddToSet(r => r.Invitations, invited.Id);

        var room = await _rooms.FindOneAndUpdateAsync(r => r.Id == invite.RoomId, update);
        if (room == null)
            throw BadRequestException($"Room {invite.RoomId} does not exist");

        var identityResult = await AddInviteToUser(invited, room.Id);
        if (!identityResult.Succeeded)
            throw InternalServerError();
    }

    [HttpPost("join")]
    public async Task JoinRoom(JoinChatRoom join)
    {
        var user = await GetUser();

        var room = await _rooms.Find(r => r.Id == join.RoomId).FirstAsync();
        if (room == null)
            throw BadRequestException($"Room {join.RoomId} does not exist");

        if (room.Members.Contains(user.Id))
            return;

        if (room.Invitations == null || !room.Invitations.Contains(user.Id))
            throw UnauthorizedException();

        var update = Builders<ChatRoom>.Update
            .Push(r => r.Members, user.Id)
            .Pull(r => r.Invitations, user.Id);

        var result = await _rooms.FindOneAndUpdateAsync(r => r.Id == join.RoomId, update);
        if (result == null)
            throw BadRequestException($"Room {join.RoomId} does not exist");

        var identityResult = await AddRoomToUser(user, room.Id);
        if (!identityResult.Succeeded)
            throw InternalServerError();
    }

    [HttpPost("leave")]
    public async Task LeaveRoom(LeaveChatRoom leave)
    {
        var user = await GetUser();

        var room = await _rooms.Find(r => r.Id == leave.RoomId).FirstAsync();
        if (room == null)
            throw BadRequestException($"Room {leave.RoomId} does not exist");

        if (!room.Members.Contains(user.Id))
            throw UnauthorizedException();

        var update = Builders<ChatRoom>.Update
            .Pull(r => r.Members, user.Id);

        var options = new FindOneAndUpdateOptions<ChatRoom>
        {
            ReturnDocument = ReturnDocument.After
        };

        var result = await _rooms.FindOneAndUpdateAsync<ChatRoom>(r => r.Id == leave.RoomId, update, options);
        if (result == null)
            throw BadRequestException($"Room {leave.RoomId} does not exist");

        if (!result.Members.Any())
        {
            var deleteResult = await DeleteRoom(result.Id);
        }

        var identityResult = await RemoveRoomFromUser(user, room.Id);
        if (!identityResult.Succeeded)
            throw InternalServerError();
    }

    private async Task<OperationResult> AddRoomToUser(ApplicationUser user, Guid roomId)
    {
        user.Rooms.Add(roomId);
        user.Invitations.Remove(roomId);

        return await Users.UpdateAsync(user);
    }

    private async Task<OperationResult> RemoveRoomFromUser(ApplicationUser user, Guid roomId)
    {
        user.Rooms.Remove(roomId);

        return await Users.UpdateAsync(user);
    }

    private async Task<OperationResult> AddInviteToUser(ApplicationUser user, Guid roomId)
    {
        user.Invitations.Add(roomId);

        return await Users.UpdateAsync(user);
    }

    private async Task<DeleteResult> DeleteRoom(Guid roomId)
    {
        return await _rooms.DeleteOneAsync(room => room.Id == roomId);
    }
}