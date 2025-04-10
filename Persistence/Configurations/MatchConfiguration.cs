using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MatchDate)
               .IsRequired();

        builder.Property(x => x.IsCompleted)
               .IsRequired();

        builder.HasOne(x => x.Team1)
               .WithMany()
               .HasForeignKey(x => x.Team1Id)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Team2)
               .WithMany()
               .HasForeignKey(x => x.Team2Id)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.Team1Id, x.Team2Id, x.MatchDate })
               .IsUnique();

        builder.HasMany(x => x.MatchTeamSeasonLeagues)
               .WithOne(x => x.Match)
               .HasForeignKey(x => x.MatchId);
    }
}
