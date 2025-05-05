using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;
using RatingTracker.Domain.DTOs;

namespace RatingTracker.Infrastructure.ScraperServices;

public class GoogleScraperService : ScraperServiceBase, IScraperService
{
    private readonly ILogger _logger;

    public GoogleScraperService(HttpClient httpClient, ILogger<GoogleScraperService> logger): base(httpClient)
    {
        _logger = logger;
    }

    public async Task<SearchEngineRanks> GetSearchRanksAsync(
        string keywords,
        string targetDomain,
        int maxResults = 100,
        CancellationToken cancellationToken = default)
    {
        string query =
            $"https://www.google.co.uk/search?q={Uri.EscapeDataString(keywords)}&num={maxResults}";
        _logger.LogInformation($"Search query: {query}");

        var request = new HttpRequestMessage(HttpMethod.Get, query);

        request.Headers.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
        request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        request.Headers.Add("Accept-Language", "en-GB,en;q=0.9");
        request.Headers.Add("Referer", "https://www.google.co.uk/");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");

        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120 Safari/537.36");
        var response = await HttpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch search results");

        string html = await response.Content.ReadAsStringAsync();
        var matches = Regex.Matches(html, @"<a href=""/url\?q=(.*?)&amp;");
        var urls = matches.Select(m => HttpUtility.HtmlDecode(m.Groups[1].Value)).ToList();
        
        return new SearchEngineRanks
        {
            SearchEngine = "Google",
            Ranks = GetRanking(urls, targetDomain)
        };
        
    }
}