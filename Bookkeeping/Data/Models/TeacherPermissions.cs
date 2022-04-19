using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookkeeping.Data.Models;

[Table("teacher_permissions", Schema = BookkeepingContext.Schema)]
public sealed class TeacherPermissions
{
	[Key, Column("teacher_id")]
	public int TeacherId { get; set; }
	
	[Column("edit_teachers")]
	public bool EditTeachers { get; set; }
	[Column("edit_children")]
	public bool EditChildren { get; set; }
	[Column("edit_subjects")]
	public bool EditSubjects { get; set; }
	[Column("read_global_statistic")]
	public bool ReadGlobalStatistic { get; set; }
	[Column("is_owner")]
	public bool IsOwner { get; set; }
	
	public Teacher Teacher { get; set; }
}