using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookkeepingNasheDetstvo.Server.Models
{
    public sealed class Place
    {
        [BsonRepresentation(BsonType.ObjectId)] public string Id { get; set; }
        
        public string Name { get; set; }
    }
}