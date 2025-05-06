using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using RatingTracker.Infrastructure.ScraperServices;

namespace RatingTracker.UnitTests.Infrastructure.ScraperServices;


public class GoogleScraperServiceTests
 {
   [Fact]
    public async Task GetSearchRanksAsync_ShouldReturnCorrectRank_WhenTargetUrlIsPresent()
    {
        // Arrange
        var html = """
            <a href="/url?q=https://www.infotrack.co.uk/some-path&amp;">
        """;

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.RequestUri!.ToString().Contains("google.co.uk")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(html),
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var logger = Mock.Of<ILogger<GoogleScraperService>>();
        var service = new GoogleScraperService(httpClient, logger);

        // Act
        var result = await service.GetSearchRanksAsync("land registry searches", "infotrack.co.uk");

        // Assert
        result.SearchEngine.Should().Be("Google");
        result.Ranks.Should().Contain(1);
    }

    [Fact]
    public async Task GetSearchRanksAsync_ShouldReturnZero_WhenTargetUrlNotPresent()
    {
        // Arrange
        var html = """
            <a href="/url?q=https://someotherdomain.com/page&amp;">
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
        var logger = Mock.Of<ILogger<GoogleScraperService>>();
        var service = new GoogleScraperService(httpClient, logger);

        // Act
        var result = await service.GetSearchRanksAsync("land registry searches", "infotrack.co.uk");

        // Assert
        result.Ranks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSearchRanksAsync_ShouldLogError_OnHttpFailure()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var loggerMock = new Mock<ILogger<GoogleScraperService>>();
        var service = new GoogleScraperService(httpClient, loggerMock.Object);

        // Act
        var result = await service.GetSearchRanksAsync("land registry searches", "infotrack.co.uk");

        // Assert
        result.Ranks.Should().BeEmpty();

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to fetch")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}