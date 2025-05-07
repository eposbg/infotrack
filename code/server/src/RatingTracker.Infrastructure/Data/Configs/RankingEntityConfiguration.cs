using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RatingTracker.Domain.Entitites;

namespace RatingTracker.Infrastructure.Data.Configs;

public class RankingEntityConfiguration : IEntityTypeConfiguration<Ranking>
{
    public void Configure(EntityTypeBuilder<Ranking> builder)
    {
        builder.ToTable("Rankings");

        builder.HasKey(r => r.RaningId);
        builder.Property(r => r.SearchEngine).IsRequired();
        builder.Property(r => r.Date).IsRequired();
        builder.Property(r => r.TopRanking).IsRequired();
    }
}