using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Incremental.Common.Metrics;
using Incremental.Common.Metrics.Sinks.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thankifi.Api.Configuration.Options;
using Thankifi.Core.Application.Pipelines;
using Thankifi.Persistence.Context;

namespace Thankifi.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        var options = configuration.GetSection(CacheOptions.Cache).Get<CacheOptions>();

        return options.Strategy switch
        {
            CacheOptions.CachingStrategy.Disabled => services,
            CacheOptions.CachingStrategy.InMemory => services
                .Configure<CacheOptions>(configuration.GetSection(CacheOptions.Cache))
                .AddDistributedMemoryCache(),
            CacheOptions.CachingStrategy.Distributed when !string.IsNullOrWhiteSpace(configuration.GetConnectionString(CacheOptions.Cache))
                => services
                    .Configure<CacheOptions>(configuration.GetSection(CacheOptions.Cache))
                    .AddStackExchangeRedisCache(builder =>
                    {
                        builder.Configuration = configuration.GetConnectionString(CacheOptions.Cache);
                        builder.InstanceName = options.InstanceNaming;
                    }),
            _ => services
        };
    }

    public static IServiceCollection AddMetrics(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(MetricsOptions.Metrics).Get<MetricsOptions>();

        if (!options.Enabled)
        {
            return services;
        }

        services.AddCommonMetrics();

        var connectionString = configuration.GetConnectionString(MetricsOptions.Metrics);
        var metricsCertificates = GetConnectionCertificates(configuration, MetricsOptions.Metrics);

        services.ConfigureEntityFrameworkSink<MetricsDbContext>(builder =>
        {
            builder.UseNpgsql(connectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly("Thankifi.Persistence.Migrations");

                optionsBuilder.ProvideClientCertificatesCallback(clientCerts => { clientCerts.AddRange(metricsCertificates); });
            });
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsPipeline<,>));

        return services;
    }

    private static X509Certificate[] GetConnectionCertificates(IConfiguration configuration, string connection)
    {
        return configuration.GetSection("Certificates").GetValue<string>(connection)
            .Split("-----END CERTIFICATE----------BEGIN CERTIFICATE-----")
            .Select(certificate => certificate
                .Replace("-----BEGIN CERTIFICATE-----", "")
                .Replace("-----END CERTIFICATE-----", ""))
            .Select(certificate => new X509Certificate(Convert.FromBase64String(certificate)))
            .ToArray();
    }
}