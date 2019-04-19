namespace BookkeepingNasheDetstvo.Client.Models.Statistic
{
    public sealed class StatisticItemModel
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public string ImageUrl { get; set; }
        
        public TotalHoursModel TotalHours { get; set; }
    }
}