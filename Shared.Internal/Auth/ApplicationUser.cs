using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Netrunner.Shared.Internal.Auth
{
    public class ApplicationUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Username { get; set; }
        public string NormalizedUsername { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public List<string> Roles { get; set; } = new();

        public ApplicationUser()
        {
        }

        public ApplicationUser(string username)
        {
            Username = username;
            NormalizedUsername = username.ToUpperInvariant();
        }
    }
}