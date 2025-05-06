using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace RatingTracker.UnitTests.Infrastructure.SearchEngineCrawlers;

public class BingScraperService
{
    [Fact]
    public async Task GetSearchRanksAsync_ShouldReturnCorrectRank_WhenTargetUrlIsPresent()
    {
        // Arrange
        var html = """
            <li class="b_algo">
              <h2><a href="https://www.infotrack.co.uk/some-path">Some Result</a></h2>
            </li>
        """;

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(html),
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var logger = Mock.Of<ILogger<BingScraperService>>();
        var service = new BingScraperService(httpClient, logger);

        // Act
        var result = await service.GetSearchRanksAsync("land registry searches", "infotrack.co.uk");

        // Assert
        result.SearchEngine.Should().Be("Bing");
        result.Ranks.Should().Contain(1); // First result
    }

    [Fact]
    public async Task GetSearchRanksAsync_ShouldReturnZero_WhenTargetUrlNotPresent()
    {
        // Arrange
        var html = """
            <li class="b_algo">
              <h2><a href="https://someotherdomain.com/page">Unrelated Result</a></h2>
            </li>
        """;

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(html),
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var logger = Mock.Of<ILogger<BingScraperService>>();
        var service = new BingScraperService(httpClient, logger);

        // Act
        var result = await service.GetSearchRanksAsync("land registry searches", "infotrack.co.uk");

        // Assert
        result.Ranks.Should().ContainSingle().Which.Should().Be(0);
    }

    [Fact]
    public async Task GetSearchRanksAsync_ShouldLogError_OnException()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Bing is down"));

        var httpClient = new HttpClient(handlerMock.Object);
        var loggerMock = new Mock<ILogger<BingScraperService>>();

        var service = new BingScraperService(httpClient, loggerMock.Object);

        // Act
        var result = await service.GetSearchRanksAsync("land registry searches", "infotrack.co.uk");

        // Assert
        result.Ranks.Should().BeEmpty();
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Bing is down")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
}