using Microsoft.Extensions.Logging;
using RatingTracker.Domain.DTOs;
using RatingTracker.Domain.SearchEngines;
using RatingTracker.Infrastructure.ScraperServices;

namespace RatingTracker.Application.Services;

public class SearchService(IScraperFactory scrapperFactory, ILogger<SearchService> logger)
    : ISearchService
{
    public async Task<SearchResult> SearchAsync(string keywords, string targetDomain, int maxResults,
        CancellationToken token = default)
    {


        var tasks = Enum.GetValues<SearchEngineType>().Select(
            async engine =>
            {
                var scraper = scrapperFactory.Create(engine);
                return await scraper.GetSearchRanksAsync(keywords, targetDomain, maxResults, token);
            });

        var engineRankings = await Task.WhenAll(tasks);

        var result = new SearchResult();

        foreach (var engineRanking in engineRankings)
        {
            result.Ranks.Add(engineRanking);
        }

        return result;
    }
}