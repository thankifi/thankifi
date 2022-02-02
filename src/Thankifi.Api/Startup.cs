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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Thankifi.Api.Configuration;
using Thankifi.Api.Configuration.Authorization;
using Thankifi.Api.Configuration.Swagger;
using Thankifi.Common.Filters;
using Thankifi.Common.Importer;
using Thankifi.Core.Application.Import;
using Thankifi.Core.Application.Import.Hosted;
using Thankifi.Core.Application.Pipelines;
using Thankifi.Core.Domain.Contract.Gratitude.Queries;
using Thankifi.Core.Domain.Gratitude.Query;
using Thankifi.Persistence.Context;

namespace Thankifi.Api;

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
        
        services.AddCache(Configuration);
        
        services.AddMetrics(Configuration);

        services.AddSourcing(typeof(RetrieveById).Assembly, typeof(RetrieveByIdHandler).Assembly);

        services.AddFilters();

        services.ConfigurePipeline(Configuration);

        services.AddImporter(default);
        
        services.AddScoped<ImportService>();
        
        services.AddHostedService<ImportHostedService>();

        services.AddThankifiContext(Configuration);

        services.ConfigureIpRateLimiting(Configuration);
        
        services.AddApiVersioning(o =>
        {
            o.ReportApiVersions = true;
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
        });
        
      
        services.AddAuthentication(o => { o.DefaultScheme = nameof(ManagementAuthenticationScheme); })
            .AddScheme<ManagementAuthenticationScheme, ManagementAuthenticationHandler>(nameof(ManagementAuthenticationScheme),
                _ => { });
        
        services.AddHealthChecks().AddDbContextCheck<ThankifiDbContext>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSerilogRequestLogging();
      
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        // else
        // {
        //     app.UseHttpsRedirection();
        // }

        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
        {
            serviceScope?.ServiceProvider.GetRequiredService<ThankifiDbContext>().Database.Migrate();
            serviceScope?.ServiceProvider.GetRequiredService<MetricsDbContext>().Database.Migrate();
        }

        app.UseStaticFiles();

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Thankifi V1");
            c.RoutePrefix = string.Empty;
            c.DocumentTitle = "Thankifi API Documentation";
            c.HeadContent = AnalyticsHeadContent.Content;
        });

        app.UseRouting();

        app.UseAuthorization();

        app.UseIpRateLimiting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}