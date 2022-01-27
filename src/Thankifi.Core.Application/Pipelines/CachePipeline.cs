using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Pagination;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Category.Queries;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;
using Thankifi.Core.Domain.Contract.Language.Dto;

namespace Thankifi.Core.Application.Pipelines;

public partial class CachePipeline
{
    private readonly IDistributedCache _cache;

    private static readonly DistributedCacheEntryOptions DefaultCacheEntryOptions =
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) };

    public CachePipeline(IDistributedCache cache)
    {
        _cache = cache;
    }

    private async Task<TItem?> RetrieveAsync<TKey, TItem>(TKey key, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{key?.GetType().FullName}:{JsonSerializer.Serialize(key)}";

        var item = await _cache.GetAsync(cacheKey, cancellationToken);

        return item is not null ? JsonSerializer.Deserialize<TItem>(item) : default;
    }

    private async Task StoreAsync<TKey, TItem>(TKey key, TItem item, DistributedCacheEntryOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{key?.GetType().FullName}:{JsonSerializer.Serialize(key)}";
        var cacheItem = JsonSerializer.SerializeToUtf8Bytes(item);
        
        await _cache.SetAsync(cacheKey, cacheItem, options ?? DefaultCacheEntryOptions, cancellationToken);
    }
}