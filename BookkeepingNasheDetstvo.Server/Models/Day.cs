using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookkeepingNasheDetstvo.Server.Models
{
    public sealed class Day
    {
        [Required] public List<IdNamePair> Teachers { get; set; }
        
        [Required] public List<IdNamePair> Children { get; set; }
        
        [Required] public List<Subject> Subjects { get; set; }
    }
}
