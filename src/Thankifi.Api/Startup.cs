using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using AspNetCoreRateLimit;
using Incremental.Common.Metrics;
using Incremental.Common.Metrics.Sinks.EntityFramework;
using Incremental.Common.Sourcing;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
using Thankifi.Core.Domain.Contract.Gratitude.Queries;
using Thankifi.Core.Domain.Gratitude.Query;
using Thankifi.Persistence.Context;

namespace Thankifi.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();

            services.AddOptions();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            #region Cache

            // required by IPRateLimiting
            services.AddMemoryCache();
            
            var cacheConnectionString = Configuration["CACHE_CONNECTION_STRING"];

            if (string.IsNullOrWhiteSpace(cacheConnectionString))
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = cacheConnectionString;
                    options.InstanceName = "thankifi_api_query_cache";
                });
            }
            
            services.Scan(scanner => scanner
                .FromAssembliesOf(typeof(CachePipeline))
                .AddClasses(filter => filter.Where(type => type == typeof(CachePipeline)))
                .AsImplementedInterfaces()
            );

            #endregion
            
            #region Metrics

            services.AddCommonMetrics();

            var metricsCertificates = Configuration["METRICS_DB_CONNECTION_CERTIFICATE"]?
                .Split("-----END CERTIFICATE----------BEGIN CERTIFICATE-----")
                .Select(certificate => certificate
                    .Replace("-----BEGIN CERTIFICATE-----", "")
                    .Replace("-----END CERTIFICATE-----", ""))
                .Select(certificate => new X509Certificate(Convert.FromBase64String(certificate)))
                .ToArray() ?? Array.Empty<X509Certificate>();
            
            services.ConfigureEntityFrameworkSink<MetricsDbContext>(builder =>
            {
                var connectionString = Configuration["METRICS_DB_CONNECTION_STRING"];
                builder.UseNpgsql(connectionString, optionsBuilder =>
                {
                    optionsBuilder.MigrationsAssembly("Thankifi.Persistence.Migrations");

                    optionsBuilder.ProvideClientCertificatesCallback(clientCerts =>
                    {
                        clientCerts.AddRange(metricsCertificates);
                    });
                });
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsPipeline<,>));
            
            #endregion
            
            #region Sourcing 

            services.AddSourcing(typeof(RetrieveById).Assembly, typeof(RetrieveByIdHandler).Assembly);

            services.Scan(scanner => scanner
                .FromAssembliesOf(typeof(FlavouringPipeline))
                .AddClasses(filter => filter.Where(type => type == typeof(FlavouringPipeline)))
                .AsImplementedInterfaces()
            );

            #endregion

            services.AddFilters();

            services.AddImporter(default);

            services.AddScoped<ImportService>();

            services.AddHostedService<ImportHostedService>();

            #region Persistence
            
            var dbCertificates = Configuration["METRICS_DB_CONNECTION_CERTIFICATE"]?
                .Split("-----END CERTIFICATE----------BEGIN CERTIFICATE-----")
                .Select(certificate => certificate
                    .Replace("-----BEGIN CERTIFICATE-----", "")
                    .Replace("-----END CERTIFICATE-----", ""))
                .Select(certificate => new X509Certificate(Convert.FromBase64String(certificate)))
                .ToArray() ?? Array.Empty<X509Certificate>();

            services.AddDbContext<ThankifiDbContext>(builder =>
            {
                var connectionString = Configuration["DB_CONNECTION_STRING"];
                builder.UseNpgsql(connectionString, optionsBuilder =>
                {
                    optionsBuilder.MigrationsAssembly("Thankifi.Persistence.Migrations");
                    
                    optionsBuilder.ProvideClientCertificatesCallback(clientCerts =>
                    {
                        clientCerts.AddRange(dbCertificates);
                    });
                });
            });

            #endregion

            #region IpRateLimiting

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            services.AddInMemoryRateLimiting();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            #endregion

            #region Api Versioning

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            #endregion

            #region SwaggerGen

            AnalyticsHeadContent.Configure(Configuration);

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

            services.AddHealthChecks()
                .AddDbContextCheck<ThankifiDbContext>();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                serviceScope?.ServiceProvider.GetRequiredService<ThankifiDbContext>().Database.Migrate();
                serviceScope?.ServiceProvider.GetRequiredService<MetricsDbContext>().Database.Migrate();
            }

            app.UseSerilogRequestLogging();

            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Thankifi V1");
                c.RoutePrefix = string.Empty;
                c.DocumentTitle = "Thankify API Documentation";
                c.HeadContent = AnalyticsHeadContent.Content;
            });

            app.UseRouting();

            app.UseIpRateLimiting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}