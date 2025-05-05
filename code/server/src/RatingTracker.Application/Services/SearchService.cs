using RatingTracker.Domain.SearchEngines;
using RatingTracker.Infrastructure.SearchEngineCrawler;

namespace RatingTracker.Application.Services;

public class SearchService(ISearchEngineCrawlerFactory searchEngineCrawlerFactory) : ISearchService
{
    public async Task<int> SearchAsync(string keywords, string targetDomain, int maxResults,
        CancellationToken token = default)
    {
        var crawler = searchEngineCrawlerFactory.Create(SearchEngineType.Google);
        var result  = await crawler.GetSearchRanksAsync(keywords, targetDomain, maxResults, token);

        return 0;
    }
}