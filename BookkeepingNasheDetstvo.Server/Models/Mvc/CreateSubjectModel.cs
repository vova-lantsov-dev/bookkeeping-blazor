using System.ComponentModel.DataAnnotations;

namespace BookkeepingNasheDetstvo.Server.Models.Mvc
{
    public sealed class CreateSubjectModel
    {
        public bool IsConsultation { get; set; }
        
        [Required] public string PlaceIdentifier { get; set; }
        
        [Required] public string Date { get; set; }
        
        [Required] public string Time { get; set; }
        
        [Required] public IdNamePair Owner { get; set; }
    }
}