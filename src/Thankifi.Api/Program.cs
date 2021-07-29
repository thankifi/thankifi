using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Thankifi.Common.Logging;

namespace Thankifi.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var assembly = Assembly.GetCallingAssembly().GetName().Name;
            
            try
            {
                var host = CreateHostBuilder(args).Build();
                
                if (Log.Logger is not null)
                {
                    Log.Information("Starting {Service}", assembly);
                }

                host.Run();
            }
            catch (Exception ex)
            {
                if (Log.Logger is not null)
                {
                    Log.Fatal(ex, "{Service} terminated unexpectedly", assembly);
                }
            }
            finally
            {
                
                if (Log.Logger is not null)
                {
                    Log.CloseAndFlush();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.AddCommonLogging();
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.Local.json", true);
                    builder.AddEnvironmentVariables();
                });
    }
}