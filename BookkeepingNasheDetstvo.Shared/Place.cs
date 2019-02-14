using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookkeepingNasheDetstvo.Shared
{
    public class Place
    {
        [BsonRepresentation(BsonType.ObjectId)] public string Id { get; set; }
        public string Name { get; set; }
    }
}