namespace RatingTracker.Application.Services;

public interface ISearchService
{
    Task<int> SearchAsync(string keywords, string targetDomain, int maxResults, CancellationToken token = default);
}