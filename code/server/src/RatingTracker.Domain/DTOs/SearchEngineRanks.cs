namespace RatingTracker.Domain.DTOs;

public class SearchEngineRanks
{
    public required string SearchEngine { get; set; }
    public List<int> Ranks { get; set; } = new(); 
}