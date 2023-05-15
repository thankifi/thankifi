using System;
using System.IO;
using System.Reflection;
using AspNetCoreRateLimit;
using Incremental.Common.Configuration;
using Incremental.Common.Logging;
using Incremental.Common.Sourcing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Thankifi.Api.Configuration.Swagger;
using Thankifi.Common.Filters;
using Thankifi.Common.Importer;
using Thankifi.Core.Application.Import;
using Thankifi.Core.Application.Import.Hosted;
using Thankifi.Core.Application.Pipelines;
using Thankifi.Core.Domain.Category.Query;
using Thankifi.Core.Domain.Contract.Category.Queries;
using Thankifi.Persistence.Context;

namespace Thankifi.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddCommonConfiguration();
        builder.Host.UseCommonLogging();

        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMemoryCache();

        builder.Services.AddOptions();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        builder.Services.AddFilters();
        builder.Services.AddImporter(default);
        builder.Services.AddScoped<ImportService>();
        builder.Services.AddHostedService<ImportHostedService>();
        
        #region Cache
        
        var cacheConnectionString = builder.Configuration["CACHE_CONNECTION_STRING"];

        if (string.IsNullOrWhiteSpace(cacheConnectionString))
        {
            builder.Services.AddDistributedMemoryCache();
        }
        else
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheConnectionString;
                options.InstanceName = "thankifi_api_query_cache";
            });
        }

        #endregion
        
        #region Sourcing

        builder.Services.AddSourcing(typeof(RetrieveById).Assembly, typeof(RetrieveByIdHandler).Assembly);

        builder.Services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(CachePipeline))
            .AddClasses(filter => filter.Where(type => type == typeof(CachePipeline)))
            .AsImplementedInterfaces()
        );

        builder.Services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(FlavouringPipeline))
            .AddClasses(filter => filter.Where(type => type == typeof(FlavouringPipeline)))
            .AsImplementedInterfaces()
        );

        #endregion

        #region Persistence

        builder.Services.AddDbContext<ThankifiDbContext>(options =>
        {
            var connectionString = builder.Configuration["DB_CONNECTION_STRING"];
            options.UseNpgsql(connectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly("Thankifi.Persistence.Migrations");
            });
        });

        #endregion
        
        #region IpRateLimiting

        //load general configuration from appsettings.json
        builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

        builder.Services.AddInMemoryRateLimiting();

        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        #endregion
        
        #region Api Versioning

        builder.Services.AddApiVersioning(o =>
        {
            o.ReportApiVersions = true;
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
        });

        #endregion
        
        #region SwaggerGen

        AnalyticsHeadContent.Configure(builder.Configuration);

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "🙏 Thankifi — Being Thankful as a Service ",
                Version = "v1",
                Description = "Thankifi is a free (as in speech) API that makes it easy for you to express gratitude. Be grateful!",
                Contact = new OpenApiContact
                {
                    Name = "Lucas Maximiliano Marino",
                    Email = string.Empty,
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

        #endregion
        
        #region HealthChecks

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ThankifiDbContext>();

        #endregion
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (app.Environment.IsProduction() || app.Environment.IsDevelopment())
        {
            using var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope();
            serviceScope?.ServiceProvider.GetRequiredService<ThankifiDbContext>().Database.Migrate();
        }

        app.UseRouting();
        app.UseIpRateLimiting();
        
        app.UseSerilogRequestLogging();

        app.UseStaticFiles();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Thankifi V1");
            c.RoutePrefix = string.Empty;
            c.DocumentTitle = "Thankifi API Documentation";
            c.HeadContent = AnalyticsHeadContent.Content;
        });
        
        app.MapControllers();
        app.MapHealthChecks("/health");

        app.Run();
    }
}