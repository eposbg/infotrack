using Microsoft.Extensions.Logging;
using RatingTracker.Domain.DTOs;

namespace RatingTracker.Infrastructure.ScraperServices;

public abstract class ScraperServiceBase
{
    private protected readonly HttpClient HttpClient;

    public ScraperServiceBase(HttpClient httpClient)
    {
        HttpClient = httpClient;
        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120 Safari/537.36");
    }

    protected SearchResult GetRanking(List<string> urlsFound, string targetDomain)
    {
        var result = new SearchResult();
         
        for (int i = 0; i < urlsFound.Count(); i++)
        {
            if (urlsFound[i].Contains(targetDomain, StringComparison.OrdinalIgnoreCase))
            {
                result.Ranks.Add(i + 1); // 1-indexed
            }
        }

        if (!result.Ranks.Any())
            result.Ranks.Add(0);

        return result;
    }
}