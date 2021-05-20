using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Netrunner.Shared.Chat
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string RoomId { get; set; }

        public string SenderId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}