using System.Collections.Generic;

namespace BookkeepingNasheDetstvo.Server.Models
{
    public sealed class Statistic
    {
        public string Name { get; set; }
        
        public int TotalHours { get; set; }
        
        public decimal PerHour { get; set; }
        
        public decimal PerHourGroup { get; set; }
        
        public List<SourceItem> SourceItems { get; set; }
    }

    public sealed class SourceItem
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public int TotalHours { get; set; }
        
        public string ImageUrl { get; set; }
        
        public bool IsConsultation { get; set; }
    }
}
