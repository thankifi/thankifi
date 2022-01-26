using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Metrics.Scope;
using Incremental.Common.Sourcing.Abstractions.Queries;
using MediatR;
using Thankifi.Core.Application.Entity;

namespace Thankifi.Core.Application.Pipelines;

public class MetricsPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IQuery<TResponse>
{
    private readonly IMetricScopeFactory _metricScopeFactory;

    public MetricsPipeline(IMetricScopeFactory metricScopeFactory)
    {
        _metricScopeFactory = metricScopeFactory;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        await using (await _metricScopeFactory.CreateScopeAsync<UsageMetricEvent>(request.GetType().FullName!, @event =>
                     {
                         @event.Parameters = JsonSerializer.Serialize(request);
                     }, cancellationToken))
        {
            return await next();
        }
    }
}