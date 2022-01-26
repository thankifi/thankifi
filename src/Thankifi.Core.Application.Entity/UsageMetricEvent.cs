using Incremental.Common.Metrics.Events;

namespace Thankifi.Core.Application.Entity;

public class UsageMetricEvent : MetricEvent
{
    public string Parameters { get; set; }
}