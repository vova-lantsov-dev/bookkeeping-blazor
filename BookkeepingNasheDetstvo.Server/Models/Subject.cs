using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using MongoDB.Bson;

namespace BookkeepingNasheDetstvo.Server.Models
{
    public class Subject
    {
        [BsonRepresentation(BsonType.ObjectId)] public string Id { get; set; }
        
        public string Date { get; set; }
        
        public string Time { get; set; }
        
        public List<IdNamePair> Children { get; set; }
        
        public IdNamePair Owner { get; set; }
        
        public string PlaceIdentifier { get; set; }
        
        [BsonIgnoreIfDefault] public bool IsConsultation { get; set; }
    }
}