using Microsoft.EntityFrameworkCore;
using Thankify.Core.Entity;

namespace Thankify.Persistence.Context
{
    public class ThankifyDbContext : DbContext
    {
        public DbSet<Gratitude> Gratitudes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<GratitudeCategory> GratitudeCategories { get; set; }
        public DbSet<ImportVersion> Version { get; set; }

        protected ThankifyDbContext()
        {
        }

        public ThankifyDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ThankifyDbContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}