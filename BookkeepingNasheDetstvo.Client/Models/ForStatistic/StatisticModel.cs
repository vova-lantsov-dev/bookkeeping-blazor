using System.Collections.Generic;

namespace BookkeepingNasheDetstvo.Client.Models.ForStatistic
{
    public sealed class StatisticModel
    {
        public decimal PerHourPrivate { get; set; }
        
        public decimal PerHourGroup { get; set; }

        public decimal PerHourConsultation { get; set; }
        
        public string Name { get; set; }
        
        public List<StatisticItemModel> Items { get; set; }
        
        public TotalHoursModel TotalHours { get; set; }
    }
}