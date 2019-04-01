namespace BookkeepingNasheDetstvo.Server.Models
{
    public sealed class PostTeacherPasswordModel
    {
        public string NewPassword { get; set; }
        
        public string TeacherId { get; set; }
    }
}