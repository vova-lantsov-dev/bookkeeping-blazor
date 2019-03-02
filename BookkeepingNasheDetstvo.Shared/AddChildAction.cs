namespace BookkeepingNasheDetstvo.Shared
{
    public class AddChildAction
    {
        public IdNamePair Owner { get; set; }
        public IdNamePair Child { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
