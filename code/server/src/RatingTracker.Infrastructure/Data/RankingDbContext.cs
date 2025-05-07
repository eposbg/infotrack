using Microsoft.EntityFrameworkCore;
using RatingTracker.Domain.Entitites;

namespace RatingTracker.Infrastructure.Data;

public class RankingDbContext : DbContext
{
    public RankingDbContext(DbContextOptions<RankingDbContext> options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RankingDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
 
    public DbSet<Ranking> Rankings { get; set; }
}