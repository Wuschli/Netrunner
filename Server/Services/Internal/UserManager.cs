using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Netrunner.Server.Configs;
using Netrunner.Shared.Internal;
using Netrunner.Shared.Internal.Auth;
using WampSharp.V2;

namespace Netrunner.Server.Services.Internal
{
    public class UserManager : IUserManager
    {
        private readonly IMongoCollection<ApplicationUser> _users;

        public UserManager(NetrunnerConfig config)
        {
            var mongoClient = new MongoClient(config.Database.ConnectionString);
            var database = mongoClient.GetDatabase(config.Database.DatabaseName);
            _users = database.GetCollection<ApplicationUser>(config.Database.UsersCollectionName);
        }

        public async Task<ApplicationUser?> GetCurrentUser()
        {
            var context = WampInvocationContext.Current?.InvocationDetails;
            var userId = context?.CallerAuthenticationId;
            if (userId == null)
                return null;

            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            return user;
        }

        public async Task<OperationResult> UpdateAsync(ApplicationUser user)
        {
            var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return new OperationResult
            {
                Succeeded = result.IsAcknowledged
            };
        }

        public async Task<ApplicationUser?> FindByNameAsync(string username)
        {
            var normalizedUsername = username.ToUpperInvariant();
            return await _users.Find(u => u.NormalizedUsername == normalizedUsername).FirstOrDefaultAsync();
        }
    }
}