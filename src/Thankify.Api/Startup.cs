using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AspNetCoreRateLimit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Thankify.Core.Domain.Gratitude.Dto;
using Thankify.Core.Domain.Gratitude.Pipeline;
using Thankify.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitude;
using Thankify.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitudeById;
using Thankify.Core.Domain.Gratitude.Query.GetBulkGratitude;
using Thankify.Core.Domain.Gratitude.Query.GetGratitude;
using Thankify.Core.Domain.Gratitude.Query.GetGratitudeById;
using TaaS.Infrastructure.Implementation.Client;
using TaaS.Infrastructure.Implementation.Service;
using Thankify.Api.Configuration.Swagger;
using Thankify.Api.Hosted;
using Thankify.Infrastructure.Contract.Client;
using Thankify.Infrastructure.Contract.Service;
using Thankify.Persistence.Context;

namespace Thankify.Api
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
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddMemoryCache();

            services.AddHostedService<ImportHostedService>();

            #region Pipeline

            services.AddMediatR(typeof(GetGratitudeByIdQuery).Assembly);
            services.AddTransient<IPipelineBehavior<GetGratitudeQuery, GratitudeDto?>, GratitudeFilterPipeline>();
            services.AddTransient<IPipelineBehavior<GetGratitudeQuery, GratitudeDto?>, GratitudeCustomizationPipeline>();

            services.AddTransient<IPipelineBehavior<GetGratitudeByIdQuery, GratitudeDto?>, GratitudeFilterPipeline>();
            services.AddTransient<IPipelineBehavior<GetGratitudeByIdQuery, GratitudeDto?>, GratitudeCustomizationPipeline>();

            services.AddTransient<IPipelineBehavior<GetBulkGratitudeQuery, IEnumerable<GratitudeDto>>, GratitudeFilterPipeline>();
            services.AddTransient<IPipelineBehavior<GetBulkGratitudeQuery, IEnumerable<GratitudeDto>>, GratitudeCustomizationPipeline>();
            
            services.AddTransient<IPipelineBehavior<GetBulkAllFiltersGratitudeQuery, IEnumerable<GratitudeDto>>, GratitudeFilterPipeline>();
            services.AddTransient<IPipelineBehavior<GetBulkAllFiltersGratitudeQuery, IEnumerable<GratitudeDto>>, GratitudeCustomizationPipeline>();
            
            services.AddTransient<IPipelineBehavior<GetBulkAllFiltersGratitudeByIdQuery, IEnumerable<GratitudeDto>>, GratitudeFilterPipeline>();
            services.AddTransient<IPipelineBehavior<GetBulkAllFiltersGratitudeByIdQuery, IEnumerable<GratitudeDto>>, GratitudeCustomizationPipeline>();
            
            #endregion
            
            #region Persistence

            services.AddDbContext<ThankifyDbContext>(builder =>
            { 
                var connectionString = Configuration["DB_CONNECTION_STRING"];
                builder.UseNpgsql(connectionString, optionsBuilder => { optionsBuilder.MigrationsAssembly("Thankify.Persistence.Migration"); });
            });

            #endregion

            #region Infrastructure

            services.AddHttpClient<IImporterClient, ImporterClient>()
                .AddTransientHttpErrorPolicy(builder =>
                    builder.WaitAndRetryAsync(3, retryCount =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryCount))));

            services.AddScoped<IImporterService, ImporterService>();

            #endregion
            
            #region IpRateLimiting

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Thanks as a Service", 
                    Version = "v1",
                    Description = "TaaS is a public API that makes it easy for you to express gratitude. Be grateful!",
                    Contact = new OpenApiContact
                    {
                      Name  = "Lucas Maximiliano Marino",
                      Email = string.Empty,
                      Url = new Uri("https://lucasmarino.me")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "GPL-3.0",
                        Url = new Uri("https://github.com/elementh/taas/blob/master/LICENSE")
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
                .AddDbContextCheck<ThankifyDbContext>();

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

            #region Migration
            
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<ThankifyDbContext>().Database.Migrate();
            }
            
            #endregion
            
            app.UseStaticFiles();
            
            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaaS V1");
                c.RoutePrefix = string.Empty;
                c.DocumentTitle = "Thanks as a Service API Docs";
                if (!string.IsNullOrWhiteSpace(Configuration["G_ANALYTICS"]))
                {
                    c.HeadContent = GAnalyticsHeader.GetHeader(Configuration["G_ANALYTICS"]);
                }
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