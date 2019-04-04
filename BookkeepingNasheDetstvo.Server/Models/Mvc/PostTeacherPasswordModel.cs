using System.ComponentModel.DataAnnotations;

namespace BookkeepingNasheDetstvo.Server.Models.Mvc
{
    public sealed class PostTeacherPasswordModel
    {
        [Required] public string NewPassword { get; set; }
        
        [Required] public string TeacherId { get; set; }
    }
}