using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookkeeping.Data.Models;

[Table("teachers", Schema = BookkeepingContext.Schema)]
public class Teacher
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public int Id { get; set; }
        
    [Column("per_hour")]
    public decimal PerHour { get; set; }
    [Column("per_hour_group")]
    public decimal PerHourGroup { get; set; }
    [Required, Column("first_name")]
    public string FirstName { get; set; }
    [Required, Column("last_name")]
    public string LastName { get; set; }
    [Required, Column("patronymic")]
    public string Patronymic { get; set; }
    [Required, Column("phone_number")]
    public string PhoneNumber { get; set; }
    [Required, Column("email")]
    public string Email { get; set; }
    [Column("additional")]
    public string? Additional { get; set; }
    [Column("image_url")]
    public string? ImageUrl { get; set; }
    
    [Column("teacher_auth_id")]
    public string AuthUserName { get; set; }
        
    [ForeignKey(nameof(Id))]
    public TeacherPermissions Permissions { get; set; }
}