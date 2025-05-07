namespace RatingTracker.Domain.DTOs;

public class SearchEngineRanks
{
    public required string SearchEngine { get; set; }
    public required string Keywords { get; set; }
    public required string TargetDomain { get; set; }
    public List<int> Ranks { get; set; } = new();
    
}