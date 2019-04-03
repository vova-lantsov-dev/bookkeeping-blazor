using System.ComponentModel.DataAnnotations;

namespace BookkeepingNasheDetstvo.Server.Models.Mvc
{
    public sealed class AddSubjectChildModel
    {
        [Required] public string Date { get; set; }

        [Required] public string Time { get; set; }

        [Required] public IdNamePair Owner { get; set; }
        
        [Required] public IdNamePair Child { get; set; }
    }
}