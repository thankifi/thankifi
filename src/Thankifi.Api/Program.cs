using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Thankifi.Api.Configuration;

namespace Thankifi.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = ConfigurationExtension.LoadLogger(Configuration);

            try
            {
                var host = CreateHostBuilder(args).Build();

                Log.Information("Starting TaaS");

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "TaaS terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfiguration Configuration { get; } = ConfigurationExtension.LoadConfiguration();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
    }
}