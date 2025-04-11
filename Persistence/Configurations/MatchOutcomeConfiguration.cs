using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MatchOutcomeConfiguration : IEntityTypeConfiguration<MatchOutcome>
{
    public void Configure(EntityTypeBuilder<MatchOutcome> builder)
    {
        builder.HasKey(x => new { x.MatchId, x.TeamId, x.OutcomeId });

        builder.HasOne(x => x.Match)
               .WithMany(x => x.Outcomes)
               .HasForeignKey(x => x.MatchId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Team)
               .WithMany()
               .HasForeignKey(x => x.TeamId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Outcome)
               .WithMany(x => x.MatchOutcomes)
               .HasForeignKey(x => x.OutcomeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
