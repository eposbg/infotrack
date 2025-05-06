using RatingTracker.Domain.DTOs;

namespace RatingTracker.Application.Services;

public interface ISearchService
{
    Task<SearchResult> SearchAsync(string keywords, string targetDomain, int maxResults, CancellationToken token = default);
}