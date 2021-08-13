using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Thankifi.Core.Application.Import.Hosted
{
    public class ImportHostedService : BackgroundService
    {
        private readonly ILogger<ImportHostedService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer? _timer;

        public ImportHostedService(ILogger<ImportHostedService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await ScheduleImportTimer(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled error importing dataset");
            }
        }

        private Task ScheduleImportTimer(CancellationToken cancellationToken)
        {
            _timer = new Timer(TimeSpan.FromHours(1).TotalMilliseconds)
            {
                AutoReset = true
            };
            
            _logger.LogInformation("Scheduling import with an interval of {Interval} seconds", TimeSpan.FromMilliseconds(_timer.Interval).TotalSeconds);
            
            _timer.Elapsed += async (_, _ ) =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                
                var importService = scope.ServiceProvider.GetRequiredService<ImportService>();

                await importService.TryImport(cancellationToken);
            };

            _timer.Disposed += (_, _) =>
            {
                _logger.LogCritical("Import timer was disposed");
            };
                
            _timer.Start();
            
            _logger.LogInformation("Import scheduled");
            
            return Task.CompletedTask;
        }
    }
}