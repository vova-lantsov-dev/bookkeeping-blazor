namespace BookkeepingNasheDetstvo.Client.Models
{
    public sealed class ChildModel
    {
        public string Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string SecondName { get; set; }
        
        public string LastName { get; set; }
        
        public string ImageUrl { get; set; }
        
        public string FatherName { get; set; }
        
        public string FatherPhone { get; set; }
        
        public string MotherName { get; set; }
        
        public string MotherPhone { get; set; }
        
        public decimal PerHour { get; set; }
        
        public decimal PerHourGroup { get; set; }
    }
}