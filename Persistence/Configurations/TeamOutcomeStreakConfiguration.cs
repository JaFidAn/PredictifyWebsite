using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class TeamOutcomeStreakConfiguration : IEntityTypeConfiguration<TeamOutcomeStreak>
{
    public void Configure(EntityTypeBuilder<TeamOutcomeStreak> builder)
    {
        builder.HasKey(x => new { x.TeamId, x.OutcomeId, x.MatchId });

        builder.Property(x => x.StreakCount)
            .IsRequired();

        builder.Property(x => x.MaxStreak)
            .IsRequired();

        builder.Property(x => x.Ratio)
            .HasColumnType("decimal(4,3)")
            .IsRequired();

        builder.Property(x => x.MatchDate)
            .IsRequired();

        builder.HasOne(x => x.Team)
            .WithMany()
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Outcome)
            .WithMany()
            .HasForeignKey(x => x.OutcomeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Match)
            .WithMany()
            .HasForeignKey(x => x.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}