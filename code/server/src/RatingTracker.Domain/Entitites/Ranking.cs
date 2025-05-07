namespace RatingTracker.Domain.Entitites;

public class Ranking
{
    public Guid RaningId { get; set; }
    public required string SearchEngine { get; set; }
    public required DateTime Date { get; set; }
    public required int TopRanking { get; set; }
    public required string Keywords { get; set; }
    public required string TargetDomain { get; set; }
    
}