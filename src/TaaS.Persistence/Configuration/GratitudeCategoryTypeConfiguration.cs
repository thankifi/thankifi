using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaaS.Core.Entity;

namespace TaaS.Persistence.Configuration
{
    public class GratitudeCategoryTypeConfiguration : IEntityTypeConfiguration<GratitudeCategory>
    {
        public void Configure(EntityTypeBuilder<GratitudeCategory> builder)
        {
            builder.HasKey(e =>
            new {e.CategoryId, e.GratitudeId });
            
            builder.HasOne(e => e.Category)
                .WithMany(e => e.Gratitudes)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Gratitude)
                .WithMany(e => e.Categories)
                .HasForeignKey(e => e.GratitudeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}