using Microsoft.EntityFrameworkCore;
using TaaS.Core.Entity;

namespace TaaS.Persistence
{
    public class TaaSDbContext : DbContext
    {
        public DbSet<Gratitude> Gratitudes { get; set; }

        protected TaaSDbContext()
        {
        }

        public TaaSDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}