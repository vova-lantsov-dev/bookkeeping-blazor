using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable InconsistentNaming

namespace Bookkeeping.Data.Models;

[Table("subjects__children", Schema = BookkeepingContext.Schema)]
public sealed class Subject__Child
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
	public int Id { get; set; }
	
	[Column("child_id")]
	public int ChildId { get; set; }
	[ForeignKey(nameof(ChildId))]
	public Child Child { get; set; }
	
	[Column("subject_id")]
	public int SubjectId { get; set; }
	[ForeignKey(nameof(SubjectId))]
	public Subject Subject { get; set; }
}