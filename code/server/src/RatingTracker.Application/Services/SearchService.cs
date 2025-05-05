using System.Text.Json;
using Microsoft.Extensions.Logging;
using RatingTracker.Domain.SearchEngines;
using RatingTracker.Infrastructure.ScraperServices;

namespace RatingTracker.Application.Services;

public class SearchService(IScraperFactory scrapperFactory, ILogger<SearchService> logger)
    : ISearchService
{
    public async Task<int> SearchAsync(string keywords, string targetDomain, int maxResults,
        CancellationToken token = default)
    {
        var scraper = scrapperFactory.Create(SearchEngineType.Bing);
        var result = await scraper.GetSearchRanksAsync(keywords, targetDomain, maxResults, token);
        logger.LogInformation(JsonSerializer.Serialize(result));

        return 0;
    }
}