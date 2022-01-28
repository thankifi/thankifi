using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

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