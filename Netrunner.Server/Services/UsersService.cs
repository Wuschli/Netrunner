using System.Security.Claims;
using MongoDB.Driver;
using Netrunner.Server.Helpers;
using Netrunner.Shared;
using Netrunner.Shared.Users;

namespace Netrunner.Server.Services;

public interface IUsersService
{
    Task<ApplicationUser?> GetUser(Guid userId);
    Task<ApplicationUser> CreateUser(Guid userId);
    Task<ApplicationUser> GetOrCreateUser(Guid userId);
    Task<OperationResult> UpdateAsync(ApplicationUser user);
}

public class UsersService : IUsersService
{
    private readonly IMongoCollection<ApplicationUser> _users;

    public UsersService(IConfiguration config)
    {
        var mongoClient = new MongoClient(config["DB:ConnectionString"]);
        var database = mongoClient.GetDatabase(config["DB:DatabaseName"]);
        _users = database.GetCollection<ApplicationUser>(Constants.UsersCollection);
    }

    public async Task<ApplicationUser?> GetUser(Guid userId)
    {
        return await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
    }

    public async Task<ApplicationUser> CreateUser(Guid userId)
    {
        var user = new ApplicationUser(userId, userId.ToString());
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<ApplicationUser> GetOrCreateUser(Guid userId)
    {
        var user = await GetUser(userId);
        if (user != null)
            return user;
        return await CreateUser(userId);
    }

    public async Task<OperationResult> UpdateAsync(ApplicationUser user)
    {
        var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        return new OperationResult
        {
            Succeeded = result.IsAcknowledged
        };
    }
}