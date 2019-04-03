using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BookkeepingNasheDetstvo.Server.Models.Mvc
{
    public sealed class DateRangeModel
    {
        [FromQuery(Name = "from"), DataType(DataType.Date)] public DateTime From { get; set; }
        
        [FromQuery(Name = "to"), DataType(DataType.Date)] public DateTime To { get; set; }
    }
}