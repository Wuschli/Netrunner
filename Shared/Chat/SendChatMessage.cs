using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Netrunner.Shared.Chat
{
    public class SendChatMessage
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string RoomId { get; set; }

        public string Message { get; set; }
    }
}