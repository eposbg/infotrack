using RatingTracker.Domain.DTOs;

namespace RatingTracker.Application.Services;

public interface IRankingService
{
    Task<SearchResult> SearchAsync(string keywords, string targetDomain, int maxResults,
        CancellationToken token = default);
    Task StoreResultsAsync(SearchResult result, CancellationToken token = default);
    Task<List<RankingDto>> GetWeeklyResultsAsync(string keywords, CancellationToken token = default);
    Task<List<RankingDto>> GetMontlyResultsAsync(string keywords, CancellationToken token = default);
}