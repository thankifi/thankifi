using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thankifi.Core.Entity;

namespace Thankifi.Persistence.Configuration
{
    public class GratitudeTypeConfiguration : IEntityTypeConfiguration<Gratitude>
    {
        public void Configure(EntityTypeBuilder<Gratitude> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Language)
                .WithMany(e => e.Gratitudes)
                .IsRequired();
            
            builder.Property(e => e.Text)
                .IsRequired();

            builder.HasMany(e => e.Categories)
                .WithMany(e => e.Gratitudes);

        }
    }
}