namespace RatingTracker.Domain.DTOs;

public class RankingDto
{
    public required string SearchEngine { get; set; }
    public DateTime Date { get; set; }
    public int TopRanking { get; set; }
}