using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using AspNetCoreRateLimit;
using Incremental.Common.Metrics;
using Incremental.Common.Metrics.Sinks.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Thankifi.Api.Configuration.Options;
using Thankifi.Api.Configuration.Swagger;
using Thankifi.Core.Application.Pipelines;
using Thankifi.Persistence.Context;

namespace Thankifi.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        var options = GetCacheOptions(configuration);

        if (options.Strategy == CacheOptions.CachingStrategy.Disabled)
            return services;
        if (options.Strategy == CacheOptions.CachingStrategy.InMemory)
            return services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Cache)).AddDistributedMemoryCache();
        if (options.Strategy == CacheOptions.CachingStrategy.Distributed &&
            !string.IsNullOrWhiteSpace(configuration.GetConnectionString(CacheOptions.Cache)))
            return services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Cache))
                .AddStackExchangeRedisCache(builder =>
                {
                    builder.Configuration = configuration.GetConnectionString(CacheOptions.Cache);
                    builder.InstanceName = options.InstanceNaming;
                });
        return services;
    }

    public static IServiceCollection AddMetrics(this IServiceCollection services, IConfiguration configuration)
    {
        var options = GetMetricsOptions(configuration);

        if (!options.Enabled)
        {
            return services;
        }

        services.AddCommonMetrics();

        var connectionString = configuration.GetConnectionString(MetricsOptions.Metrics);
        var certificates = GetConnectionCertificates(configuration, MetricsOptions.Metrics);

        services.ConfigureEntityFrameworkSink<MetricsDbContext>(builder =>
        {
            builder.UseNpgsql(connectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly("Thankifi.Persistence.Migrations");

                optionsBuilder.ProvideClientCertificatesCallback(certs => { certs.AddRange(certificates); });
            });
        });

        return services;
    }

    public static IServiceCollection ConfigurePipeline(this IServiceCollection services, IConfiguration configuration)
    {
        if (GetMetricsOptions(configuration).Enabled)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsPipeline<,>));
        }

        if (GetCacheOptions(configuration).Strategy != CacheOptions.CachingStrategy.Disabled)
        {
            services.Scan(scanner => scanner
                .FromAssembliesOf(typeof(CachePipeline))
                .AddClasses(filter => filter.Where(type => type == typeof(CachePipeline)))
                .AsImplementedInterfaces()
            );
        }

        services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(FlavouringPipeline))
            .AddClasses(filter => filter.Where(type => type == typeof(FlavouringPipeline)))
            .AsImplementedInterfaces()
        );

        return services;
    }

    public static IServiceCollection AddThankifiContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Thankifi");
        var certificates = GetConnectionCertificates(configuration, MetricsOptions.Metrics);

        services.AddDbContext<ThankifiDbContext>(builder =>
        {
            builder.UseNpgsql(connectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly("Thankifi.Persistence.Migrations");

                optionsBuilder.ProvideClientCertificatesCallback(certs => { certs.AddRange(certificates); });
            });
        });

        return services;
    }

    public static IServiceCollection ConfigureIpRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        
        services.AddInMemoryRateLimiting();
        
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }

    public static IServiceCollection AddOpenApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        AnalyticsHeadContent.Configure(configuration);
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "🙏 Thankifi — Being Thankful as a Service ",
                Version = "v1",
                Description = "Thankifi is a free (as in speech) API that makes it easy for you to express gratitude. Be grateful!",
                Contact = new OpenApiContact
                {
                    Name = "Lucas Maximiliano Marino",
                    Email = "hello@lucasmarino.me",
                    Url = new Uri("https://lucasmarino.me")
                },
                License = new OpenApiLicense
                {
                    Name = "AGPL-3.0",
                    Url = new Uri("https://github.com/thankifi/thankifi/blob/master/LICENSE")
                }
            });
        
            c.OperationFilter<RemoveVersionFromParameter>();
            c.DocumentFilter<ReplaceVersionWithExactValueInPath>();
        
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    private static MetricsOptions GetMetricsOptions(IConfiguration configuration)
    {
        return configuration.GetSection(MetricsOptions.Metrics).Get<MetricsOptions>();
    }

    private static CacheOptions GetCacheOptions(IConfiguration configuration)
    {
        return configuration.GetSection(CacheOptions.Cache).Get<CacheOptions>();
    }

    private static X509Certificate[] GetConnectionCertificates(IConfiguration configuration, string connection)
    {
        return configuration[$"Certificates:{connection}"]?
            .Split("-----END CERTIFICATE----------BEGIN CERTIFICATE-----")
            .Select(certificate => certificate
                .Replace("-----BEGIN CERTIFICATE-----", "")
                .Replace("-----END CERTIFICATE-----", ""))
            .Select(certificate => new X509Certificate(Convert.FromBase64String(certificate)))
            .ToArray() ?? Array.Empty<X509Certificate>();
    }
}