using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MatchTeamSeasonLeagueConfiguration : IEntityTypeConfiguration<MatchTeamSeasonLeague>
{
    public void Configure(EntityTypeBuilder<MatchTeamSeasonLeague> builder)
    {
        builder.HasKey(x => new { x.MatchId, x.TeamId, x.SeasonId, x.LeagueId });

        builder.HasOne(x => x.Match)
               .WithMany(x => x.MatchTeamSeasonLeagues)
               .HasForeignKey(x => x.MatchId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Team)
               .WithMany()
               .HasForeignKey(x => x.TeamId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Season)
               .WithMany()
               .HasForeignKey(x => x.SeasonId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.League)
               .WithMany()
               .HasForeignKey(x => x.LeagueId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
