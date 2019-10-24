namespace BookkeepingNasheDetstvo.Client.Models.ForSubject
{
    public sealed class SubjectModel
    {
        public string Id { get; set; }
        
        public string OwnerId { get; set; }
        
        public string Time { get; set; }
        
        public bool IsGroup { get; set; }
        
        public bool IsConsultation { get; set; }
    }
}