using System;
using System.Diagnostics.Eventing.Reader;

namespace Thankifi.Api.Configuration.Options;

public class CacheOptions
{
    public static string Cache => "Cache";

    public CacheOptions()
    {
        Strategy = CachingStrategy.Disabled;
        Lifetime = TimeSpan.FromDays(7);
        InstanceNaming = "thankifi_query_cache";
    }
    
    public CachingStrategy Strategy { get; set; }
    public string InstanceNaming { get; set; }
    public TimeSpan Lifetime { get; set; }

    public enum CachingStrategy
    {
        Disabled,
        InMemory,
        Distributed
    }
}