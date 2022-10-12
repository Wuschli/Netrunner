using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Netrunner.ServerLegacy.Data.Challenges
{
    internal class ChallengeInstance
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ResourceId { get; set; }

        public DateTime Deadline { get; set; }
        public ChallengeData ChallengeData { get; set; }
        public IList<ChallengeStepData>? Steps { get; set; }
    }

    /// <summary>
    /// Challenge that is represented to the user
    /// </summary>
    internal class ChallengeData
    {
    }

    /// <summary>
    /// Steps that are committed for the solution of the challenge
    /// </summary>
    internal class ChallengeStepData
    {
    }
}