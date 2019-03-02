using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookkeepingNasheDetstvo.Shared
{
    public sealed class Credential
    {
        [BsonRepresentation(BsonType.ObjectId)] public string Id;

        public string Salt;

        public string PasswordHash;
    }
}