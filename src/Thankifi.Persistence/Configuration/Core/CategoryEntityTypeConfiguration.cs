using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thankifi.Core.Entity;

namespace Thankifi.Persistence.Configuration.Core;

public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Slug)
            .IsRequired();

        builder.HasMany(e => e.Gratitudes)
            .WithMany(e => e.Categories);

        builder.HasIndex(e => e.Slug)
            .IsUnique();
    }
}