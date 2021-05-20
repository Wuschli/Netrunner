using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Netrunner.Shared.Users;

namespace Netrunner.Server.Identity.Data
{
    public class ApplicationUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Username { get; set; }
        public List<string>? Rooms { get; set; }
        public List<string>? Invitations { get; set; }
        public List<Contact>? Contacts { get; set; }

        public ApplicationUser()
        {
        }

        public ApplicationUser(string username)
        {
            Username = username;
        }
    }
}