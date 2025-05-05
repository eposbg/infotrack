namespace RatingTracker.Infrastructure.SearchEngineCrawler;

public interface ISearchEngineCrawler
{
    Task<List<int>> GetSearchRanksAsync(string keywords, string targetDomain, int maxResults = 100, CancellationToken token = default);
}


