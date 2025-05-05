using Microsoft.AspNetCore.Mvc;
using RatingTracker.Application.Services;
using RatingTracker.Domain.DTOs;

namespace RatingTracker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController(ISearchService searchService) : ControllerBase
{
    // GET
    [Route("")]
    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchQueryDto query)
    {
        var result = await searchService.SearchAsync(query.Keywords, query.TargetDomain, query.MaxResults ?? 100, HttpContext.RequestAborted);
        return Ok(result);
    }
}