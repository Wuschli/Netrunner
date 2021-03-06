using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Netrunner.Shared.Chat
{
    public class ChatRoom
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public ICollection<string> Members { get; set; }
        public ICollection<string> Invitations { get; set; }
    }
}