using System.ComponentModel.DataAnnotations;
using BookkeepingNasheDetstvo.Server.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookkeepingNasheDetstvo.Server.Models
{
    public class Child
    {
        [BsonRepresentation(BsonType.ObjectId)] public string Id { get; set; }
        [Required] public string FirstName { get; set; }
        
        [Required] public string SecondName { get; set; }
        
        [Required] public string LastName { get; set; }
        
        [Required] public string ImageUrl { get; set; }
        
        [Required] public string FatherName { get; set; }
        
        [Required] public string FatherPhone { get; set; }
        
        [Required] public string MotherName { get; set; }
        
        [Required] public string MotherPhone { get; set; }
        
        [BsonSerializer(typeof(CustomMongoDecimalSerializer))] public decimal PerHour { get; set; }
        
        [BsonSerializer(typeof(CustomMongoDecimalSerializer))] public decimal PerHourGroup { get; set; }
    }
}