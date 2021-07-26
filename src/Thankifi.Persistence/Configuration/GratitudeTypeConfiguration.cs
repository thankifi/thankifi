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
        }
    }
}