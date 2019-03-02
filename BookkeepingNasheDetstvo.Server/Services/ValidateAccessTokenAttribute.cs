using BookkeepingNasheDetstvo.Server.Models;
using BookkeepingNasheDetstvo.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BookkeepingNasheDetstvo.Server.Services
{
    public class ValidateAccessTokenAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Token", out var authToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var dbContext = (BookkeepingContext) context.HttpContext.RequestServices.GetService(typeof(BookkeepingContext));
            var session = await dbContext.Sessions.Find(s => s.Token == authToken[0]).FirstOrDefaultAsync();
            if (session == default)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var teacher = await dbContext.Teachers.Find(Builders<Teacher>.Filter.Eq(t => t.Id, session.OwnerId)).FirstOrDefaultAsync();
            if (teacher == default)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            
            if (context.ActionArguments.ContainsKey("current"))
                context.ActionArguments["current"] = teacher;
            await next();
        }
    }
}
