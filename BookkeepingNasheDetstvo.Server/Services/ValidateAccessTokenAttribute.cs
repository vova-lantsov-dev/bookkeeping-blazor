using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace BookkeepingNasheDetstvo.Server.Services
{
    public class ValidateAccessTokenAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Auth-Token", out var token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var cancellationToken = context.HttpContext.RequestAborted;
            var bookkeepingContext = context.HttpContext.RequestServices.GetService<BookkeepingContext>();
            var session = await bookkeepingContext.Sessions.Find(s => s.Token == token)
                .SingleOrDefaultAsync(cancellationToken);
            if (session == default)
            {
                context.Result = new StatusCodeResult(403);
                return;
            }

            if (context.ActionArguments.ContainsKey("current"))
                context.ActionArguments["current"] = await bookkeepingContext.Teachers.Find(
                    t => t.Id == session.OwnerId).SingleAsync(cancellationToken);
        }
    }
}
