using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BookkeepingNasheDetstvo.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment(EnvironmentName.Development)
                .UseKestrel()
                .UseShutdownTimeout(TimeSpan.FromSeconds(20d))
                .UseSockets()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:25000")
                .Build().RunAsync();
        }
    }
}
