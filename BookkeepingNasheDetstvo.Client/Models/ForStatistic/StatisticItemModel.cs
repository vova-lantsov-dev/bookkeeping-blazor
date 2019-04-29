namespace BookkeepingNasheDetstvo.Client.Models.ForStatistic
{
    public sealed class StatisticItemModel
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public string ImageUrl { get; set; }
        
        public TotalHoursModel TotalHours { get; set; }
    }
}