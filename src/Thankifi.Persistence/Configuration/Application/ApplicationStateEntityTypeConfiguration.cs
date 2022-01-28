using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thankifi.Core.Application.Entity;

namespace Thankifi.Persistence.Configuration.Application;

public class ApplicationStateEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationState>
{
    public void Configure(EntityTypeBuilder<ApplicationState> builder)
    {
        builder.Property(typeof(Guid), "Id")
            .HasDefaultValue(Guid.Parse("24c372ce-a7c2-4895-8241-3d8d432f61b7"));

        builder.HasKey("Id");

        builder.Property(e => e.DatasetVersion);
            
        builder.Property(e => e.LastUpdated);
    }
}