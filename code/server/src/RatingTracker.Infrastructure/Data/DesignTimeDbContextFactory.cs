using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RatingTracker.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RankingDbContext>
{
    public RankingDbContext CreateDbContext(string[] args)
    { 
        var basePath = Path.Combine(Directory.GetCurrentDirectory());
        Console.WriteLine(basePath);
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath) 
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        
        Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
        var optionsBuilder = new DbContextOptionsBuilder<RankingDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        
        return new RankingDbContext(optionsBuilder.Options);
    }
}