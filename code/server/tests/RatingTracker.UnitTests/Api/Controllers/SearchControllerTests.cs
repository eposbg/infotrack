using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RatingTracker.Application.Services;
using RatingTracker.Domain.DTOs;
using RatingTracker.WebApi.Controllers;

namespace RatingTracker.UnitTests.Api.Controllers;

public class SearchControllerTests
{
    [Fact]
    public async Task Search_ShouldReturn200Ok_WithSearchResult()
    {
        // Arrange
        var query = new SearchQueryDto
        {
            Keywords = "land registry",
            TargetDomain = "infotrack.co.uk",
            MaxResults = 100
        };

        var fakeResult = new SearchResult
        {
            Ranks = new List<SearchEngineRanks>
            {
                new SearchEngineRanks { SearchEngine = "Google", Ranks = new List<int> { 1, 10 } },
                new SearchEngineRanks { SearchEngine = "Bing", Ranks = new List<int> { 3 } }
            }
        };

        var serviceMock = new Mock<ISearchService>();
        serviceMock.Setup(s => s.SearchAsync(query.Keywords, query.TargetDomain, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResult);

        var loggerMock = new Mock<ILogger<SearchController>>();
        var controller = new SearchController(serviceMock.Object, loggerMock.Object);
        using var cts = new CancellationTokenSource();
        var expectedToken = cts.Token;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestAborted = expectedToken
            }
        };

        // Act
        var response = await controller.Search(query);

        // Assert
        var okResult = response.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeAssignableTo<SearchResult>().Subject;

        returnedResult.Ranks.Should().HaveCount(2);
        returnedResult.Ranks.Should().Contain(r => r.SearchEngine == "Google");
        returnedResult.Ranks.Should().Contain(r => r.SearchEngine == "Bing");
    }

    [Fact]
    public async Task Search_ShouldLogSearchQuery()
    {
        // Arrange
        var query = new SearchQueryDto
        {
            Keywords = "land registry",
            TargetDomain = "infotrack.co.uk",
            MaxResults = 50
        };

        var serviceMock = new Mock<ISearchService>();
        serviceMock.Setup(s => s.SearchAsync(query.Keywords, query.TargetDomain, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchResult());

        var loggerMock = new Mock<ILogger<SearchController>>();
        var controller = new SearchController(serviceMock.Object, loggerMock.Object);
        using var cts = new CancellationTokenSource();
        var expectedToken = cts.Token;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestAborted = expectedToken
            }
        };


        // Act
        await controller.Search(query);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) =>
                    o.ToString()!.Contains("Keywords") && o.ToString()!.Contains("Target Domain")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}