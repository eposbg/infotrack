using System.Text.RegularExpressions;

namespace RatingTracker.Infrastructure.SearchEngineCrawler;

public class GoogleCrawler : ISearchEngineCrawler
{
    private readonly HttpClient _httpClient;

    public GoogleCrawler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<int>> GetSearchRanksAsync(
        string keywords,
        string targetDomain,
        int maxResults = 100,
        CancellationToken cancellationToken = default)
    {
        // string query = $"https://www.google.co.uk/search?num=100&q={Uri.EscapeDataString(keywords)}";
        string query =
            $"https://www.google.co.uk/search?q={Uri.EscapeDataString(keywords)}&num={maxResults}&sca_esv=ad6373a997a56455&sxsrf=AHTn8zpFrZ67mkJ1SY0AEJHRI1LYdQIIfQ%3A1746217776810&ei=MCsVaIuiMdGKkdUP-sPkGQ&ved=0ahUKEwiL-qWS0IWNAxVRRaQEHfohOQMQ4dUDCBA&uact=5&oq=land+registry+search&gs_lp=Egxnd3Mtd2l6LXNlcnAiFGxhbmQgcmVnaXN0cnkgc2VhcmNoMgoQABiwAxjWBBhHMgoQABiwAxjWBBhHMgoQABiwAxjWBBhHMgoQABiwAxjWBBhHMgoQABiwAxjWBBhHMgoQABiwAxjWBBhHMgoQABiwAxjWBBhHMgoQABiwAxjWBBhHMg0QABiABBiwAxhDGIoFMg0QABiABBiwAxhDGIoFSMQGUABYAHABeAGQAQCYAQCgAQCqAQC4AQPIAQCYAgGgAgqYAwCIBgGQBgqSBwExoAcAsgcAuAcA&sclient=gws-wiz-serp";


        var request = new HttpRequestMessage(HttpMethod.Get, query);
        request.Headers.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
        request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        request.Headers.Add("Accept-Language", "en-GB,en;q=0.9");
        request.Headers.Add("Referer", "https://www.google.co.uk/");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch search results");

        string html = await response.Content.ReadAsStringAsync();

        // Extract all result URLs from <a href="..."> tags
        var matches = Regex.Matches(html, @"<a href=""/url\?q=(https?[^&]+)&");

        var ranks = new List<int>();
        int position = 1;

        foreach (Match match in matches)
        {
            var url = match.Groups[1].Value;
            if (url.Contains(targetDomain, StringComparison.OrdinalIgnoreCase))
            {
                ranks.Add(position);
            }

            position++;
        }

        return ranks.Count > 0 ? ranks : new List<int> { 0 }; // return 0 if not found
    }
}