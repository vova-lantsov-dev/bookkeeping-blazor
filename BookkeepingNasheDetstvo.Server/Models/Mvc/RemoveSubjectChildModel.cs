using System.ComponentModel.DataAnnotations;

namespace BookkeepingNasheDetstvo.Server.Models.Mvc
{
    public sealed class RemoveSubjectChildModel
    {
        [Required] public string Id { get; set; }
        
        [Required] public string ChildId { get; set; }
    }
}