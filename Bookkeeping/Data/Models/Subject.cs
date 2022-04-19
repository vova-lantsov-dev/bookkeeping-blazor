using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookkeeping.Data.Models;

[Table("subjects", Schema = BookkeepingContext.Schema)]
public sealed class Subject
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public int Id { get; set; }
    
    [Column("date")]
    public DateOnly Date { get; set; }
    [Column("time")]
    public string Time { get; set; }
    [Column("is_consultation")]
    public bool IsConsultation { get; set; }
        
    [ForeignKey(nameof(LocationId))]
    public Location Location { get; set; }
    [Column("location_id")]
    public int LocationId { get; set; }
        
    [ForeignKey(nameof(TeacherId))]
    public Teacher Teacher { get; set; }
    [Column("teacher_id")]
    public int TeacherId { get; set; }
        
    public List<Subject__Child> ChildrenReference { get; set; }
}