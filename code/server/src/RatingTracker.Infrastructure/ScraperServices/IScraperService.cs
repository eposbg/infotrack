using RatingTracker.Domain.DTOs;

namespace RatingTracker.Infrastructure.ScraperServices;

public interface IScraperService
{
    Task<SearchEngineRanks> GetSearchRanksAsync(string keywords, string targetDomain, int maxResults = 100, CancellationToken token = default);
}


