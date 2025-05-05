using RatingTracker.Domain.SearchEngines;

namespace RatingTracker.Infrastructure.SearchEngineCrawler;

public interface ISearchEngineCrawlerFactory
{
    ISearchEngineCrawler Create(SearchEngineType searchEngineType);
}

public class SearchEngineCrawlerFactory(): ISearchEngineCrawlerFactory
{
    public ISearchEngineCrawler Create(SearchEngineType searchEngineType)
    {
        switch (searchEngineType)
        {
            case SearchEngineType.Google: 
                return new GoogleCrawler(new HttpClient());
            default:
                throw new NotSupportedException($"Search Engine Type {searchEngineType} is not supported.");
        }
    }
}