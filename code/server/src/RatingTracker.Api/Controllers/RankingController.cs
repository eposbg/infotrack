using Microsoft.AspNetCore.Mvc;
using RatingTracker.Application.Services;
using RatingTracker.Domain.DTOs;

namespace RatingTracker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RankingController(
    IRankingService rankingService,
    ILogger<RankingController> logger) : ControllerBase
{
    [Route("")]
    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchQueryDto query)
    {
        logger.LogInformation(
            $"Search request (Keywords: \"{query.Keywords}\", Target Domain \"{query.TargetDomain}\")");
        var result = await rankingService.SearchAsync(query.Keywords, query.TargetDomain, query.MaxResults ?? 100,
            HttpContext.RequestAborted);
        return Ok(result);
    }

    [HttpGet]
    [Route("monthly/history/{keywords}/{targetDomain}")]
    public async Task<IActionResult> GetMontlyHistory(string keywords, string targetDomain)
    {
        logger.LogInformation($"GetMontlyHistory by keywords {keywords}");
        var results = await rankingService.GetHistoricalRatingsAsync(keywords, targetDomain,
            DateTime.Today.AddMonths(-1), HttpContext.RequestAborted);
        return Ok(results);
    }

    [HttpGet]
    [Route("weekly/history/{keywords}/{targetDomain}")]
    public async Task<IActionResult> GetWeeklyHistory(string keywords, string targetDomain)
    {
        logger.LogInformation($"GetWeeklyHistory keywords {keywords}");
        var results =
            await rankingService.GetHistoricalRatingsAsync(keywords, targetDomain, DateTime.Today.AddDays(-7),
                HttpContext.RequestAborted);
        return Ok(results);
    }
}