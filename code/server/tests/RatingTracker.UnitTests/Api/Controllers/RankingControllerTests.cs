using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RatingTracker.Application.Services;
using RatingTracker.Domain.DTOs;
using RatingTracker.WebApi.Controllers;

namespace RatingTracker.UnitTests.Api.Controllers;

public class RankingControllerTests
{
    private readonly Mock<IRankingService> _rankingServiceMock;
    private readonly Mock<ILogger<RankingController>> _loggerMock;
    private readonly RankingController _controller;
    
    public RankingControllerTests()
    {
        _rankingServiceMock = new Mock<IRankingService>();
        _loggerMock = new Mock<ILogger<RankingController>>();
        _controller = new RankingController(_rankingServiceMock.Object, _loggerMock.Object);
        using var cts = new CancellationTokenSource();
        var expectedToken = cts.Token;
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestAborted = expectedToken
            }
        };
    }
    
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
                new SearchEngineRanks { SearchEngine = "Google", Ranks = new List<int> { 1, 10 }, Keywords = query.Keywords, TargetDomain = query.TargetDomain },
                new SearchEngineRanks { SearchEngine = "Bing", Ranks = new List<int> { 3 }, Keywords = query.Keywords, TargetDomain = query.TargetDomain  }
            }
        };

        _rankingServiceMock.Reset();
        _rankingServiceMock.Setup(s => s.SearchAsync(query.Keywords, query.TargetDomain, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResult);
       
        // Act
        var response = await _controller.Search(query);

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
        _loggerMock.Reset();
        _rankingServiceMock.Reset();
        _rankingServiceMock.Setup(s => s.SearchAsync(query.Keywords, query.TargetDomain, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchResult());


        // Act
        await _controller.Search(query);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) =>
                    o.ToString()!.Contains("Keywords") && o.ToString()!.Contains("Target Domain")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    
    
    [Fact]
    public async Task GetMonthlyHistory_ShouldReturnOk_WithExpectedResults()
    {
        // Arrange
        var keywords = "example keyword";
        var expectedResults = new List<RankingDto>
        {
            new RankingDto { SearchEngine = "Google", Date = DateTime.UtcNow, TopRanking = 3 }
        };

        _loggerMock.Reset();
        _rankingServiceMock.Reset();
        
        _rankingServiceMock
            .Setup(s => s.GetMontlyResultsAsync(keywords, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        // Act
        var result = await _controller.GetMontlyHistory(keywords);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expectedResults);
    }

    [Fact]
    public async Task GetWeeklyHistory_ShouldReturnOk_WithExpectedResults()
    {
        // Arrange
        var keywords = "weekly test";
        var expectedResults = new List<RankingDto>
        {
            new RankingDto { SearchEngine = "Bing", Date = DateTime.UtcNow.AddDays(-7), TopRanking = 5 }
        };

        _rankingServiceMock.Reset();
        _rankingServiceMock
            .Setup(s => s.GetWeeklyResultsAsync(keywords, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        // Act
        var result = await _controller.GetWeeklyHistory(keywords);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expectedResults);
    }
}