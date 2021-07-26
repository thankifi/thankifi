using Microsoft.EntityFrameworkCore;
using Thankifi.Core.Entity;

namespace Thankifi.Persistence.Context
{
    public class ThankifiDbContext : DbContext
    {
        public DbSet<Gratitude> Gratitudes { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected ThankifiDbContext()
        {
        }

        public ThankifiDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ThankifiDbContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}