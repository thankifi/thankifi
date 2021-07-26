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
using Thankifi.Api.Configuration.Swagger;
using Thankifi.Core.Domain.Gratitude.Dto;
using Thankifi.Core.Domain.Gratitude.Pipeline;
using Thankifi.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitude;
using Thankifi.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitudeById;
using Thankifi.Core.Domain.Gratitude.Query.GetBulkGratitude;
using Thankifi.Core.Domain.Gratitude.Query.GetGratitude;
using Thankifi.Core.Domain.Gratitude.Query.GetGratitudeById;
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
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddMemoryCache();
            
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

            services.AddDbContext<ThankifiDbContext>(builder =>
            { 
                var connectionString = Configuration["DB_CONNECTION_STRING"];
                builder.UseNpgsql(connectionString, optionsBuilder => { optionsBuilder.MigrationsAssembly("Thankifi.Persistence.Migration"); });
            });

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
                    Title = "🙏 Thankifi — Being Thankful as a Service ", 
                    Version = "v1",
                    Description = "Thankifi is a FOSS API that makes it easy for you to express gratitude. Be grateful!",
                    Contact = new OpenApiContact
                    {
                      Name  = "Lucas Maximiliano Marino",
                      Email = string.Empty,
                      Url = new Uri("https://lucasmarino.me")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "AGPL-3.0",
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

            #region Migration
            
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                serviceScope?.ServiceProvider.GetRequiredService<ThankifiDbContext>().Database.Migrate();
            }
            
            #endregion
            
            app.UseStaticFiles();
            
            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Thankify V1");
                c.RoutePrefix = string.Empty;
                c.DocumentTitle = "Thankify API Documentation";
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