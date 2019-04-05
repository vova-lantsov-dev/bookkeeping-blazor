using System.ComponentModel.DataAnnotations;

namespace BookkeepingNasheDetstvo.Server.Models.Mvc
{
    public sealed class AddSubjectChildModel
    {
        [Required] public string Id { get; set; }
        
        [Required] public IdNamePair Child { get; set; }
    }
}