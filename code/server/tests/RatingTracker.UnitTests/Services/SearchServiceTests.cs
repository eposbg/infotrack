using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RatingTracker.Application.Services;
using RatingTracker.Domain.DTOs;
using RatingTracker.Domain.SearchEngines;
using RatingTracker.Infrastructure.ScraperServices;

namespace RatingTracker.UnitTests.Services;

public class SearchServiceTests
{
    [Fact]
    public async Task SearchAsync_ReturnsCorrectResults_WhenScrapersWork()
    {
        // Arrange
        var googleRanks = new SearchEngineRanks
        {
            SearchEngine = "Google",
            Ranks = new() { 1 }
        };
        var bingRanks = new SearchEngineRanks
        {
            SearchEngine = "Bing",
            Ranks = new() { 10 }
        };

        var googleMock = new Mock<IScraperService>();
        var bingMock = new Mock<IScraperService>();

        googleMock.Setup(x => x.GetSearchRanksAsync("keywords", "domain.com", 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleRanks);

        bingMock.Setup(x => x.GetSearchRanksAsync("keywords", "domain.com", 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bingRanks);

        var factoryMock = new Mock<IScraperFactory>();
        factoryMock.Setup(f => f.Create(SearchEngineType.Google)).Returns(googleMock.Object);
        factoryMock.Setup(f => f.Create(SearchEngineType.Bing)).Returns(bingMock.Object);

        var logger = Mock.Of<ILogger<SearchService>>();
        var service = new SearchService(factoryMock.Object, logger);

        // Act
        var result = await service.SearchAsync("keywords", "domain.com", 100);

        // Assert
        result.Ranks.Should().HaveCount(2);
        result.Ranks.Should().Contain(r => r.SearchEngine == "Google" && r.Ranks.Contains(1));
        result.Ranks.Should().Contain(r => r.SearchEngine == "Bing" && r.Ranks.Contains(10));
    }

    [Fact]
    public async Task SearchAsync_CallsEachScraperExactlyOnce()
    {
        // Arrange
        var googleMock = new Mock<IScraperService>();
        var bingMock = new Mock<IScraperService>();

        googleMock.Setup(x =>
                x.GetSearchRanksAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchEngineRanks { SearchEngine = "Google" });

        bingMock.Setup(x =>
                x.GetSearchRanksAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchEngineRanks { SearchEngine = "Bing" });

        var factoryMock = new Mock<IScraperFactory>();
        factoryMock.Setup(f => f.Create(SearchEngineType.Google)).Returns(googleMock.Object);
        factoryMock.Setup(f => f.Create(SearchEngineType.Bing)).Returns(bingMock.Object);

        var logger = Mock.Of<ILogger<SearchService>>();
        var service = new SearchService(factoryMock.Object, logger);

        // Act
        await service.SearchAsync("test", "example.com", 100);

        // Assert
        googleMock.Verify(
            s => s.GetSearchRanksAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()), Times.Once);
        bingMock.Verify(
            s => s.GetSearchRanksAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_HandlesExceptionsAndContinues()
    {
        // Arrange
        var googleMock = new Mock<IScraperService>();
        var bingMock = new Mock<IScraperService>();

        googleMock.Setup(x =>
                x.GetSearchRanksAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Google error"));

        bingMock.Setup(x =>
                x.GetSearchRanksAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchEngineRanks { SearchEngine = "Bing", Ranks = new() { 5 } });

        var factoryMock = new Mock<IScraperFactory>();
        factoryMock.Setup(f => f.Create(SearchEngineType.Google)).Returns(googleMock.Object);
        factoryMock.Setup(f => f.Create(SearchEngineType.Bing)).Returns(bingMock.Object);

        var loggerMock = new Mock<ILogger<SearchService>>();
        var service = new SearchService(factoryMock.Object, loggerMock.Object);

        // Act
        var result = await service.SearchAsync("test", "example.com", 100);

        // Assert
        result.Ranks.Should().ContainSingle(r => r.SearchEngine == "Bing");
        result.Ranks.Should().NotContain(r => r.SearchEngine == "Google");

        // Optionally check logging
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Google error")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}