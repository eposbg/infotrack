using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RatingTracker.Application.Services;
using RatingTracker.Domain.DTOs;
using RatingTracker.Domain.Repositories;
using RatingTracker.Domain.SearchEngines;
using RatingTracker.Infrastructure.ScraperServices;

namespace RatingTracker.UnitTests.Services;

public class RankingServiceTests
{
    [Fact]
    public async Task SearchAsync_ReturnsCorrectResults_WhenScrapersWork()
    {
        // Arrange
        var keywords = "fake keywords";
        var targetDomain = "fake target domain";
        var googleRanks = new SearchEngineRanks
        {
            SearchEngine = "Google",
            Ranks = new() { 1 },
            Keywords = keywords,
            TargetDomain = targetDomain
        };
        var bingRanks = new SearchEngineRanks
        {
            SearchEngine = "Bing",
            Ranks = new() { 10 },
            Keywords = keywords,
            TargetDomain = targetDomain
        };

        var googleMock = new Mock<IScraperService>();
        var bingMock = new Mock<IScraperService>();

        googleMock.Setup(x => x.GetSearchRanksAsync("keywords", "domain.com", 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleRanks);

        bingMock.Setup(x => x.GetSearchRanksAsync("keywords", "domain.com", 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bingRanks);

        var rankingRepositoryMock = new Mock<IRankingRepository>();
        var factoryMock = new Mock<IScraperFactory>();
        factoryMock.Setup(f => f.Create(SearchEngineType.Google)).Returns(googleMock.Object);
        factoryMock.Setup(f => f.Create(SearchEngineType.Bing)).Returns(bingMock.Object);

        var logger = Mock.Of<ILogger<RankingService>>();
        var service = new RankingService(factoryMock.Object, logger, rankingRepositoryMock.Object);

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
        var keywords = "fake keywords";
        var targetDomain = "fake target domain";

        googleMock.Setup(x =>
                x.GetSearchRanksAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchEngineRanks { SearchEngine = "Google", Keywords = keywords, TargetDomain = targetDomain });

        bingMock.Setup(x =>
                x.GetSearchRanksAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchEngineRanks { SearchEngine = "Bing", Keywords = keywords, TargetDomain = targetDomain });

        var rankingRepositoryMock = new Mock<IRankingRepository>();
        
        var factoryMock = new Mock<IScraperFactory>();
        factoryMock.Setup(f => f.Create(SearchEngineType.Google)).Returns(googleMock.Object);
        factoryMock.Setup(f => f.Create(SearchEngineType.Bing)).Returns(bingMock.Object);

        var logger = Mock.Of<ILogger<RankingService>>();
        var service = new RankingService(factoryMock.Object, logger, rankingRepositoryMock.Object);

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

}