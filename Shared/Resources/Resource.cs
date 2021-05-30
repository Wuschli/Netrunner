using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Netrunner.Shared.Resources
{
    public class Resource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}