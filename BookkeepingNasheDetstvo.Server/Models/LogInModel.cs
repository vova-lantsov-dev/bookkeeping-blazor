using System.ComponentModel.DataAnnotations;

namespace BookkeepingNasheDetstvo.Server.Models
{
    public class LogInModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Login { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
