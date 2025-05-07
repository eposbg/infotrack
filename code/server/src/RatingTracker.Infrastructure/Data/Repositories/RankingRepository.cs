using Microsoft.EntityFrameworkCore;
using RatingTracker.Domain.Entitites;
using RatingTracker.Domain.Repositories;

namespace RatingTracker.Infrastructure.Data.Repositories;

public class RankingRepository(RankingDbContext db) : IRankingRepository
{
    public IQueryable<Ranking> Get(DateTime now, string searchEngine, string keywords,
        string targetDomain)
    {
        return db.Rankings
            .Where(r => r.SearchEngine == searchEngine)
            .Where(x => x.TargetDomain == targetDomain)
            .Where(x => x.Keywords == keywords)
            .Where(x => x.Date.Date >= now.Date);
    }

    public IQueryable<Ranking> Get(string keywords, string targetDomain, DateTime fromDate)
    {
        return db.Rankings
            .Where(x => x.TargetDomain == targetDomain)
            .Where(x => x.Keywords == keywords)
            .Where(x => x.Date.Date >= fromDate);
    }

    public void DeleteByKeywordForDate(DateTime date, string keywords)
    {
        var existingRecords = db.Rankings
            .Where(x => x.Date.Date >= date.Date)
            .Where(x => x.Keywords == keywords);
        if (existingRecords.Any())
        {
            db.Rankings.RemoveRange(existingRecords);
        }
    }

    public async Task CreateAsync(Ranking ranking, CancellationToken token)
    {
        await db.Rankings.AddAsync(ranking, token);
    }

    public async Task SaveAsync(CancellationToken token)
    {
        await db.SaveChangesAsync(token);
    }
}