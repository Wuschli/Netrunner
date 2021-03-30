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
    }
}