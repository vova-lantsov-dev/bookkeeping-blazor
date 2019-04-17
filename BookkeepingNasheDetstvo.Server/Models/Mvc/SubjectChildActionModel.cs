using System.ComponentModel.DataAnnotations;

namespace BookkeepingNasheDetstvo.Server.Models.Mvc
{
    public sealed class SubjectChildActionModel
    {
        [Required] public string SubjectId { get; set; }
        
        [Required] public string ChildId { get; set; }
    }
}