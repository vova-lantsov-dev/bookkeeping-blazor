using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using MongoDB.Bson;

namespace BookkeepingNasheDetstvo.Server.Models
{
    public sealed class Subject
    {
        [BsonRepresentation(BsonType.ObjectId)] public string Id { get; set; }
        
        public string Date { get; set; }
        
        public string Time { get; set; }
        
        public List<string> ChildrenIds { get; set; }
        
        public string OwnerId { get; set; }
        
        public string PlaceIdentifier { get; set; }
        
        [BsonIgnoreIfDefault] public bool IsConsultation { get; set; }
    }
}