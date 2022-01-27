using Microsoft.EntityFrameworkCore;
using Thankifi.Core.Application.Entity;
using Thankifi.Core.Entity;

namespace Thankifi.Persistence.Context;

public class ThankifiDbContext : DbContext
{
    public DbSet<Gratitude> Gratitudes { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ApplicationState> ApplicationState { get; set; }

    protected ThankifiDbContext()
    {
    }

    public ThankifiDbContext(DbContextOptions<ThankifiDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ThankifiDbContext).Assembly);
            
        base.OnModelCreating(modelBuilder);
    }
}