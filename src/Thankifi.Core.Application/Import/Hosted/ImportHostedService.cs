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

        private static readonly double FirstRunInterval = TimeSpan.FromSeconds(10).TotalMilliseconds;
        private static readonly double DefaultInterval = TimeSpan.FromHours(1).TotalMilliseconds;

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
            _timer = new Timer(FirstRunInterval)
            {
                AutoReset = true
            };
            
            _logger.LogInformation("Scheduling import with an interval of {DefaultInterval} seconds", TimeSpan.FromMilliseconds(DefaultInterval).TotalSeconds);
            _logger.LogInformation("First import to start in around {FirstRunInterval} seconds", TimeSpan.FromMilliseconds(FirstRunInterval).TotalSeconds);

            _timer.Elapsed += async (_, _ ) =>
            {
                _timer.Stop();
                
                using var scope = _serviceScopeFactory.CreateScope();
                
                var importService = scope.ServiceProvider.GetRequiredService<ImportService>();

                await importService.TryImport(cancellationToken);

                if (_timer.Interval < DefaultInterval)
                {
                    _timer.Interval = DefaultInterval;
                }
                
                _timer.Start();
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