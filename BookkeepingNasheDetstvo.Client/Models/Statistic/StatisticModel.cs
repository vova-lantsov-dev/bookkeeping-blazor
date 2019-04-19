using System.Collections.Generic;

namespace BookkeepingNasheDetstvo.Client.Models.Statistic
{
    public sealed class StatisticModel
    {
        public decimal PerHour { get; set; }
        
        public decimal PerHourGroup { get; set; }
        
        public string Name { get; set; }
        
        public List<StatisticItemModel> Items { get; set; }
        
        public TotalHoursModel TotalHours { get; set; }
    }
}