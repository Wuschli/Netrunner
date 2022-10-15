using System;
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
        public Guid Id { get; set; }

        public string Username { get; set; }
        public string NormalizedUsername { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<Guid>? Rooms { get; set; }
        public List<Guid>? Invitations { get; set; }
        public List<Contact>? Contacts { get; set; }

        public ApplicationUser(string username)
        {
            Username = username;
            NormalizedUsername = username.Normalize().ToUpperInvariant();
        }
    }
}