namespace RatingTracker.Domain.DTOs;

public class SearchQueryDto
{
    public required string Keywords { get; set; }
    public required string TargetDomain { get; set; }
    public int? MaxResults { get; set; }
    
}