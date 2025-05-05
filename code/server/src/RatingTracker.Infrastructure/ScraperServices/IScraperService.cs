using RatingTracker.Domain.DTOs;

namespace RatingTracker.Infrastructure.ScraperServices;

public interface IScraperService
{
    Task<SearchResult> GetSearchRanksAsync(string keywords, string targetDomain, int maxResults = 100, CancellationToken token = default);
}


