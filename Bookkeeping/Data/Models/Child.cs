using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookkeeping.Data.Models;

[Table("children", Schema = BookkeepingContext.Schema)]
public sealed class Child
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public int Id { get; set; }
    
    [Column("per_hour")]
    public decimal PerHour { get; set; }
    [Column("per_hour_group")]
    public decimal PerHourGroup { get; set; }
    [Required, Column("first_name")]
    public string FirstName { get; set; }
    [Required, Column("patronymic")]
    public string Patronymic { get; set; }
    [Required, Column("last_name")]
    public string LastName { get; set; }
    [Column("image_url")]
    public string? ImageUrl { get; set; }
    [Column("father_name")]
    public string? FatherName { get; set; }
    [Column("father_phone")]
    public string? FatherPhone { get; set; }
    [Column("mother_name")]
    public string? MotherName { get; set; }
    [Column("mother_phone")]
    public string? MotherPhone { get; set; }
    
    public List<Subject__Child> SubjectsReference { get; set; }
}