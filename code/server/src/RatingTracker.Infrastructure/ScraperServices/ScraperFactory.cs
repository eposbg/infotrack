using Microsoft.Extensions.DependencyInjection;
using RatingTracker.Domain.SearchEngines;

namespace RatingTracker.Infrastructure.ScraperServices;

public interface IScraperFactory
{
    IScraperService Create(SearchEngineType searchEngineType);
}

public class ScraperFactory(IServiceProvider provider): IScraperFactory
{
    public IScraperService Create(SearchEngineType searchEngineType)
    {
        switch (searchEngineType)
        {
            case SearchEngineType.Google:
                return provider.GetRequiredService<GoogleScraperService>();
            case SearchEngineType.Bing: 
                return provider.GetRequiredService<BingScraperService>();
            default:
                throw new NotSupportedException($"Search Engine Type {searchEngineType} is not supported.");
        }
    }
}