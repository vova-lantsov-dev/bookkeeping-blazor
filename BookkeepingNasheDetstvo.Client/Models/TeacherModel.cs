namespace BookkeepingNasheDetstvo.Client.Models
{
    public sealed class TeacherModel
    {
        public string Id { get; set; }
        
        public decimal PerHour { get; set; }
        
        public decimal PerHourGroup { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string SecondName { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string Email { get; set; }
        
        public string Additional { get; set; }
        
        public string ImageUrl { get; set; }
        
        public bool EditTeachers { get; set; }
        
        public bool EditChildren { get; set; }
        
        public bool EditSubjects { get; set; }
        
        public bool ReadGlobalStatistic { get; set; }
        
        public bool IsOwner { get; set; }
    }
}