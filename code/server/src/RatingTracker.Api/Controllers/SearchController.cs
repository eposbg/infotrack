using Microsoft.AspNetCore.Mvc;
using RatingTracker.Application.Services;
using RatingTracker.Domain.DTOs;

namespace RatingTracker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController(ISearchService searchService, ILogger<SearchController> logger) : ControllerBase
{
    // GET
    [Route("")]
    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchQueryDto query)
    {
        logger.LogInformation($"Search request (Keywords: \"{query.Keywords}\", Target Domain \"{query.TargetDomain}\")");
        var result = await searchService.SearchAsync(query.Keywords, query.TargetDomain, query.MaxResults ?? 100, HttpContext.RequestAborted);
        return Ok(result);
    }
}