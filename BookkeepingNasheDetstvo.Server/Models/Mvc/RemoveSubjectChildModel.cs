using System.ComponentModel.DataAnnotations;

namespace BookkeepingNasheDetstvo.Server.Models.Mvc
{
    public sealed class RemoveSubjectChildModel
    {
        [Required] public string Date { get; set; }
        
        [Required] public string Time { get; set; }
        
        [Required] public string OwnerId { get; set; }
        
        [Required] public string ChildId { get; set; }
    }
}