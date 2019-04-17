using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookkeepingNasheDetstvo.Server.Models
{
    public sealed class Credential
    {
        [BsonRepresentation(BsonType.ObjectId), BsonId] public string TeacherId;

        public string Salt;

        public string PasswordHash;
    }
}