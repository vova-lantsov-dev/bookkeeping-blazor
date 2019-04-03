using System.ComponentModel.DataAnnotations;

namespace BookkeepingNasheDetstvo.Server.Models.Mvc
{
    public sealed class IdNamePair
    {
        [Required] public string Id { get; set; }
        
        [Required] public string Name { get; set; }
    }
}