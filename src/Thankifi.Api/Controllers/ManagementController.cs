using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thankifi.Api.Configuration.Authorization;
using Thankifi.Core.Application.Entity;
using Thankifi.Persistence.Context;

namespace Thankifi.Api.Controllers;

[Authorize(AuthenticationSchemes = nameof(ManagementAuthenticationScheme))]
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ManagementController : ControllerBase
{
    private readonly MetricsDbContext _dbContext;

    public ManagementController(MetricsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("metrics", Name = nameof(Metrics))]
    public async Task<IActionResult> Metrics([FromQuery] MetricsQueryStringParameters filters, CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<UsageMetricEvent>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filters.Event))
        {
            query = query.Where(e => e.Name == filters.Event);
        }

        if (!string.IsNullOrWhiteSpace(filters.Parameters))
        {
            query = query.Where(e => e.Parameters.Contains(filters.Parameters));
        }

        if (filters.After is not null)
        {
            query = query.Where(e => e.StartDate > filters.After.Value.ToUniversalTime());
        }
        if (filters.Before is not null)
        {
            query = query.Where(e => e.EndDate < filters.Before.Value.ToUniversalTime());
        }
        if (filters.Over != default)
        {
            query = query.Where(e => e.Duration > filters.Over);
        }
        if (filters.Below != default)
        {
            query = query.Where(e => e.Duration < filters.Below);
        }
        
        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(g => g.Id)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync(cancellationToken);
            
        return Ok(new PaginatedList<UsageMetricEvent>(items, count, filters.PageNumber, filters.PageSize));
    }
    
    [HttpGet("metrics/types", Name = nameof(MetricEventTypes))]
    public async Task<IActionResult> MetricEventTypes(CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<UsageMetricEvent>().AsNoTracking();

        var items = await query
            .GroupBy(e => e.Name)
            .Select(events => new
            {
                Event = events.Key,
                Count = events.Count()
            })
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    public record MetricsQueryStringParameters : QueryStringParameters
    {
        public string? Event { get; init; }
        public string? Parameters { get; init; }
        public DateTime? After { get; init; }
        public DateTime? Before { get; init; }
        public int Over { get; init; }
        public int Below { get; init; }
    }
}