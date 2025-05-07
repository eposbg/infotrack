using RatingTracker.Domain.Entitites;

namespace RatingTracker.Infrastructure.Data;

public class DbInitializer
{
    public static void Seed(RankingDbContext context)
    {
        if (!context.Rankings.Any())
        {
            var indexDate = DateTime.Today.AddMonths(-1);
            var historicalRankings = new List<Ranking>();
            var random = new Random();
            for (var today = DateTime.Today; indexDate.Date < today.Date; indexDate = indexDate.AddDays(1))
            {
                historicalRankings.Add(new Ranking
                    {
                        RaningId = Guid.CreateVersion7(),
                        Date = indexDate.Date,
                        Keywords = "News weather next 10 days",
                        TargetDomain = "metoffice.gov.uk",
                        SearchEngine = "Bing",
                        TopRanking = random.Next(1,10)
                    }
                );
            }
            
            context.Rankings.AddRange(historicalRankings);
            context.SaveChanges();
        }
    }
}