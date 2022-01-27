using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thankifi.Core.Entity;

namespace Thankifi.Persistence.Configuration.Core;

public class LanguageEntityTypeConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(e => e.Id);
            
        builder.Property(e => e.Code)
            .IsRequired();

        builder.HasMany(e => e.Gratitudes)
            .WithOne(e => e.Language);
            
        builder.HasIndex(e => e.Code)
            .IsUnique();
    }
}