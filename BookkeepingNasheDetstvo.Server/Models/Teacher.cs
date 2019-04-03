using System.ComponentModel.DataAnnotations;
using BookkeepingNasheDetstvo.Server.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookkeepingNasheDetstvo.Server.Models
{
    public class Teacher
    {
        [BsonRepresentation(BsonType.ObjectId)] public string Id { get; set; }
        [BsonSerializer(typeof(CustomMongoDecimalSerializer))] public decimal PerHour { get; set; }
        [BsonSerializer(typeof(CustomMongoDecimalSerializer))] public decimal PerHourGroup { get; set; }
        [Required(AllowEmptyStrings = true)] public string FirstName { get; set; }
        [Required(AllowEmptyStrings = true)] public string LastName { get; set; }
        [Required(AllowEmptyStrings = true)] public string SecondName { get; set; }
        [Required(AllowEmptyStrings = true)] public string PhoneNumber { get; set; }
        [Required(AllowEmptyStrings = true)] public string Email { get; set; }
        [Required(AllowEmptyStrings = true)] public string Additional { get; set; }
        [Required(AllowEmptyStrings = true)] public string ImageUrl { get; set; }
        public bool EditTeachers { get; set; }
        public bool EditChildren { get; set; }
        public bool EditSubjects { get; set; }
        public bool ReadGlobalStatistic { get; set; }
        public bool IsOwner { get; set; }
    }
}