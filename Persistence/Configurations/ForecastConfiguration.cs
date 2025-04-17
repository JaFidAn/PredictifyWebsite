using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ForecastConfiguration : IEntityTypeConfiguration<Forecast>
{
    public void Configure(EntityTypeBuilder<Forecast> builder)
    {
        builder.ToTable("Forecasts");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.StreakCount)
            .IsRequired();

        builder.Property(f => f.MaxStreak)
            .IsRequired();

        builder.Property(f => f.Ratio)
            .IsRequired()
            .HasColumnType("float");

        builder.Property(f => f.IsForecasted)
            .IsRequired();

        builder.Property(f => f.IsCorrect)
            .IsRequired(false);

        builder.HasOne(f => f.Match)
            .WithMany()
            .HasForeignKey(f => f.MatchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Team)
            .WithMany()
            .HasForeignKey(f => f.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Outcome)
            .WithMany()
            .HasForeignKey(f => f.OutcomeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}