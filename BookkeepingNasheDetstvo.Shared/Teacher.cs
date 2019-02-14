using BookkeepingNasheDetstvo.Shared.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookkeepingNasheDetstvo.Shared
{
    public class Teacher
    {
        [BsonRepresentation(BsonType.ObjectId)] public string Id { get; set; }
        [BsonSerializer(typeof(CustomMongoDecimalSerializer))] public decimal PerHour { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Additional { get; set; }
        public string ImageUrl { get; set; }
        public bool EditTeachers { get; set; }
        public bool EditChildren { get; set; }
        public bool EditSubjects { get; set; }
        public bool ReadGlobalStatistics { get; set; }
        public bool IsOwner { get; set; }
    }
}