using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Netrunner.Shared.Users;

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
        public List<string>? Rooms { get; set; }
        public List<string>? Invitations { get; set; }
        public List<Contact>? Contacts { get; set; }

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