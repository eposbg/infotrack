using System.Xml.Linq;
using RatingTracker.Domain.Entitites;

namespace RatingTracker.Domain.Repositories;

public interface IRankingRepository
{
    IQueryable<Ranking> Get(DateTime now, string searchEngine, string keywords,
        string targetDomain);

    void DeleteByKeywordForDate(DateTime date, string keywords);
    Task CreateAsync(Ranking ranking, CancellationToken token);

    Task SaveAsync(CancellationToken token);
}