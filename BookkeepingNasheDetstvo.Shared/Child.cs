using System.ComponentModel.DataAnnotations;
using BookkeepingNasheDetstvo.Shared.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookkeepingNasheDetstvo.Shared
{
    public class Child
    {
        [BsonRepresentation(BsonType.ObjectId)] public string Id { get; set; }
        [Required(AllowEmptyStrings = true)] public string FirstName { get; set; }
        [Required(AllowEmptyStrings = true)] public string SecondName { get; set; }
        [Required(AllowEmptyStrings = true)] public string LastName { get; set; }
        [Required(AllowEmptyStrings = true)] public string ImageUrl { get; set; }
        [Required(AllowEmptyStrings = true)] public string FatherName { get; set; }
        [Required(AllowEmptyStrings = true)] public string FatherPhone { get; set; }
        [Required(AllowEmptyStrings = true)] public string MotherName { get; set; }
        [Required(AllowEmptyStrings = true)] public string MotherPhone { get; set; }
        [BsonSerializer(typeof(CustomMongoDecimalSerializer))] public decimal PerHour { get; set; }
        [BsonSerializer(typeof(CustomMongoDecimalSerializer))] public decimal PerHourGroup { get; set; }
    }
}