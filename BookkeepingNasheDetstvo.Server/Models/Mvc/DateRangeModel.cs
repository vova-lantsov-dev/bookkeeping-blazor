using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BookkeepingNasheDetstvo.Server.Models.Mvc
{
    public sealed class DateRangeModel
    {
        [FromQuery(Name = "from"), Required] public string From { get; set; }
        
        [FromQuery(Name = "to"), Required] public string To { get; set; }
    }
}