using BookkeepingNasheDetstvo.Shared.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookkeepingNasheDetstvo.Shared
{
    public class Child
    {
        [BsonRepresentation(BsonType.ObjectId)] public string Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        public string FatherName { get; set; }
        public string FatherPhone { get; set; }
        public string MotherName { get; set; }
        public string MotherPhone { get; set; }
        [BsonSerializer(typeof(CustomMongoDecimalSerializer))] public decimal PerHour { get; set; }
        [BsonSerializer(typeof(CustomMongoDecimalSerializer))] public decimal PerHourGroup { get; set; }
    }
}