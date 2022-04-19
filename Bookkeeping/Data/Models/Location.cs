using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookkeeping.Data.Models;

[Table("locations", Schema = BookkeepingContext.Schema)]
public sealed class Location
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
    public int Id { get; set; }
    
    [Required, Column("name")]
    public string Name { get; set; }
}