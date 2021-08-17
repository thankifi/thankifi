using System;
using System.Reflection;
using Incremental.Common.Configuration;
using Incremental.Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

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

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.AddCommonLogging();
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddCommonConfiguration();
                });
    }
}