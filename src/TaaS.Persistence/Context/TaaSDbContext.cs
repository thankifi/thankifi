using Microsoft.EntityFrameworkCore;
using TaaS.Core.Entity;

namespace TaaS.Persistence.Context
{
    public class TaaSDbContext : DbContext
    {
        public DbSet<Gratitude> Gratitudes { get; set; }

        protected TaaSDbContext()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public TaaSDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}