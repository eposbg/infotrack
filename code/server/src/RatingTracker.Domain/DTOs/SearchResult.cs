namespace RatingTracker.Domain.DTOs;

public class SearchResult
{
    public List<SearchEngineRanks> Ranks { get; set; } = new(); 
}