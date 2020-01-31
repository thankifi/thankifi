using Microsoft.EntityFrameworkCore;
using TaaS.Core.Entity;

namespace TaaS.Persistence.Context
{
    public class TaaSDbContext : DbContext
    {
        public DbSet<Gratitude> Gratitudes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<GratitudeCategory> GratitudeCategories { get; set; }

        protected TaaSDbContext()
        {
        }

        public TaaSDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaaSDbContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}