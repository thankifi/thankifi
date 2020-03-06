using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaaS.Core.Domain.Import.Command.ImportGratitudes;

namespace TaaS.Api.WebApi.Hosted
{
    public class ImportHostedService : BackgroundService
    {
        protected readonly ILogger<ImportHostedService> Logger;
        protected readonly IServiceScopeFactory ServiceScopeFactory;
        protected readonly CronExpression Expression;
        protected System.Timers.Timer Timer;
        protected readonly TimeZoneInfo TimeZoneInfo;
        
        public ImportHostedService(ILogger<ImportHostedService> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            Logger = logger;
            ServiceScopeFactory = serviceScopeFactory;
            Expression = CronExpression.Parse(configuration["IMPORTER_CRON_CONFIGURATION"] ?? "0 0 * * *");
            TimeZoneInfo = TimeZoneInfo.Utc;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await ScheduleImport(cancellationToken);
        }

        private async Task ScheduleImport(CancellationToken cancellationToken)
        {
            Logger.LogDebug("Scheduling import job.");

            var next = Expression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo);

            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                
                Logger.LogDebug("Next import run in {Delay}", delay);
                
                Timer = new System.Timers.Timer(delay.TotalMilliseconds);
                Timer.Elapsed += async (sender, args) =>
                {
                    Timer.Close(); // Resetting

                    using (var scope = ServiceScopeFactory.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                        await mediator.Send(new ImportGratitudesCommand(), cancellationToken);
                    }
                    
                    await ScheduleImport(cancellationToken);
                };
                
                Timer.Start();
            }

            await Task.CompletedTask;
        }
    }
}