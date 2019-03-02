using BookkeepingNasheDetstvo.Server.Services;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BookkeepingNasheDetstvo.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddJsonFormatters(settings =>
            {
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }).AddDataAnnotations().AddRazorViewEngine().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });
            services.AddSingleton<BookkeepingContext>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                        Console.WriteLine(contextFeature.Error);
                    return Task.CompletedTask;
                });
            });
            app.UseResponseCompression();
            app.UseMvc();
            app.UseBlazor<Client.Startup>();
        }
    }
}