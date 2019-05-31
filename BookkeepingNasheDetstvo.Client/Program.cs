using Microsoft.AspNetCore.Blazor.Hosting;
using System;

namespace BookkeepingNasheDetstvo.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>().Build().Run();
        }
    }
}
