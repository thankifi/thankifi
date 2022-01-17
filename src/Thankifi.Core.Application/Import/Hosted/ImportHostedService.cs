using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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

        private static readonly TimeSpan FirstRunInterval = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan DefaultInterval = TimeSpan.FromHours(6);

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
            _timer = new Timer(FirstRunInterval.TotalMilliseconds)
            {
                AutoReset = true
            };
            
            _logger.LogInformation("Scheduling import with an interval of {DefaultInterval} hours", DefaultInterval.TotalHours);
            _logger.LogInformation("First import to start in around {FirstRunInterval} seconds", FirstRunInterval.TotalSeconds);

            _timer.Elapsed += OnElapsed;

            _timer.Disposed += (_, _) =>
            {
                _logger.LogCritical("Import timer was disposed");
            };
                
            _timer.Start();
            
            _logger.LogInformation("Import scheduled");
            
            return Task.CompletedTask;
            
            async void OnElapsed(object o, ElapsedEventArgs elapsedEventArgs)
            {
                _timer.Stop();

                using var scope = _serviceScopeFactory.CreateScope();

                var importService = scope.ServiceProvider.GetRequiredService<ImportService>();

                await importService.TryImport(cancellationToken);

                if (TimeSpan.FromMilliseconds(_timer.Interval) == FirstRunInterval)
                {
                    _timer.Interval = DefaultInterval.TotalMilliseconds;
                }

                _timer.Start();
            }
        }
    }
}