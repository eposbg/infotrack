using Microsoft.Extensions.Logging;
using RatingTracker.Application.Helpers;
using RatingTracker.Domain.DTOs;
using RatingTracker.Domain.Entitites;
using RatingTracker.Domain.Repositories;
using RatingTracker.Domain.SearchEngines;
using RatingTracker.Infrastructure.ScraperServices;

namespace RatingTracker.Application.Services;

public class RankingService(
    IScraperFactory scraperFactory,
    ILogger<RankingService> logger,
    IRankingRepository rankingRepository)
    : IRankingService
{
    public async Task<SearchResult> SearchAsync(string keywords, string targetDomain, int maxResults,
        CancellationToken token = default)
    {
        var tasks = Enum.GetValues<SearchEngineType>().Select(
            async engine =>
            {
                var scraper = scraperFactory.Create(engine);
                return await scraper.GetSearchRanksAsync(keywords, targetDomain, maxResults, token);
            });

        var engineRankings = await Task.WhenAll(tasks);

        var result = new SearchResult();

        foreach (var engineRanking in engineRankings)
        {
            result.Ranks.Add(engineRanking);
        }

        await StoreResultsAsync(result, token);

        return result;
    }

    public async Task StoreResultsAsync(SearchResult result, CancellationToken token = default)
    {
        foreach (var searchEngineRank in result.Ranks)
        {
            var now = DateTime.Today;
            var keywords = KeywordHelper.KeywordsCleanup(searchEngineRank.Keywords);
            var ranking = new Ranking
            {
                Date = now,
                SearchEngine = searchEngineRank.SearchEngine,
                Keywords = keywords,
                TargetDomain = searchEngineRank.TargetDomain,
                TopRanking = searchEngineRank.Ranks.Any() ? searchEngineRank.Ranks.Min() : -1
            };

            rankingRepository.DeleteByKeywordForDate(now.Date, keywords);
            await rankingRepository.CreateAsync(ranking, token);
            await rankingRepository.SaveAsync(token);
            logger.LogInformation("The ranking results are stored");
        }
    }

    public async Task<List<RankingDto>> GetWeeklyResultsAsync(string keywords, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<RankingDto>> GetMontlyResultsAsync(string keywords, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}