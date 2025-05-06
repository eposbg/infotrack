using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;
using RatingTracker.Domain.DTOs;

namespace RatingTracker.Infrastructure.ScraperServices;

public class BingScraperService : ScraperServiceBase, IScraperService
{
    private readonly ILogger _logger;

    public BingScraperService(HttpClient httpClient, ILogger<BingScraperService> logger) : base(httpClient)
    {
        _logger = logger;
    }

    public async Task<SearchEngineRanks> GetSearchRanksAsync(
        string keywords,
        string targetDomain,
        int maxResults = 100,
        CancellationToken cancellationToken = default)
    {
        var result = new SearchEngineRanks
        {
            SearchEngine = "Bing",
        };

        try
        {
            var encodedKeyword = Uri.EscapeDataString(keywords);
            var searchUrl = $"https://www.bing.com/search?q={encodedKeyword}&count=100&first=50";
            _logger.LogInformation($"Query Url {searchUrl}");
            var html = await HttpClient.GetStringAsync(searchUrl, cancellationToken);

            /*** Result element example from bing
             *
             *
             * <li class="b_algo" data-id="" iid="SERP.5967" data-bm="15">
             *   <div class="b_tpcn" style="">
             *     <a class="tilk" aria-label="HM Land Registry" redirecturl="" href="https://eservices.landregistry.gov.uk/mapsearch/addressSearch" h="ID=SERP,5454.1" target="_blank">
             *   <div class="tpic">
             *     <div class="wr_fav" data-priority="2">
             *       <div class="cico siteicon" style="width:32px;height:32px;">
             *         <img src="...">
             *       </div>
             *   </div>
             * </div>
             * <div class="tptxt">
             *   <div class="tptt">HM Land Registry</div>
             *   <div class="tpmeta">
             *     <div class="b_attribution" u="10|5117|4862500527751765|2lXl9fUmbLGjNaKkCZTPAnsp-MNxs2Gc" tabindex="0">
             *        <cite>
             *        https://eservices.landregistry.gov.uk › mapsearch › addressSearch</cite><a href="#" class="trgr_icon" aria-label="Actions for this site" aria-haspopup="true" aria-expanded="false" tabindex="0" role="button">
             *        <span class="c_tlbxTrg"><span class="c_tlbxTrgIcn sw_ddgn"></span>
             *        <span class="c_tlbxH" h="BASE:GENERATIVEQIHINTSIGNAL" k="SERP,5459.1"></span></span></a>
             *    <div class="c_tlbxLiveRegion" aria-live="polite" aria-atomic="true"></div></div></div></div></a></div>
             *        <h2 style="">
             *          <a href="https://eservices.landregistry.gov.uk/mapsearch/addressSearch" h="ID=SERP,5454.2" target="_blank" style="">HM Land Registry Portal Login</a>
             *        </h2>
             *       <div class="b_caption" style="">
             *       <p class="b_lineclamp2">Search for property ownership and land information in England and Wales using HM Land Registry's MapSearch service.
             *      </p>
             *    </div>
             *  </li>
             */

            var matches = Regex.Matches(html, @"<li class=""b_algo"".*?<h2.*?><a\s+href=""(.*?)""",
                RegexOptions.Singleline);

            var urls = matches.Select(m => HttpUtility.HtmlDecode(m.Groups[1].Value)).ToList();
            result.Ranks = GetRanking(urls, targetDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return result;
    }
}