using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class TeamSeasonLeagueConfiguration : IEntityTypeConfiguration<TeamSeasonLeague>
{
    public void Configure(EntityTypeBuilder<TeamSeasonLeague> builder)
    {
        builder.HasKey(x => new { x.TeamId, x.SeasonId, x.LeagueId });

        builder.Property(x => x.TeamId).IsRequired();
        builder.Property(x => x.SeasonId).IsRequired();
        builder.Property(x => x.LeagueId).IsRequired();

        builder.HasOne(x => x.Team)
            .WithMany(t => t.TeamSeasonLeagues)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Season)
            .WithMany(s => s.TeamSeasonLeagues)
            .HasForeignKey(x => x.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.League)
            .WithMany()
            .HasForeignKey(x => x.LeagueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
