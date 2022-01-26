using Incremental.Common.Metrics.Events;
using Incremental.Common.Metrics.Sinks.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using Thankifi.Core.Application.Entity;

namespace Thankifi.Persistence.Context;

public class MetricsDbContext : IncrementalMetricsDbContext
{
    protected MetricsDbContext()
    {
    }

    public MetricsDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UsageMetricEvent>().HasBaseType<MetricEvent>();
    }
}