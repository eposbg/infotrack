using RatingTracker.Domain.DTOs;

namespace RatingTracker.Application.Services;

public interface IRankingService
{
    Task<SearchResult> SearchAsync(string keywords, string targetDomain, int maxResults,
        CancellationToken token = default);

    Task StoreResultsAsync(SearchResult result, CancellationToken token = default);

    Task<List<RankingDto>> GetHistoricalRatingsAsync(string keywords, string targetDomain, DateTime fromDate,
        CancellationToken token = default);
}