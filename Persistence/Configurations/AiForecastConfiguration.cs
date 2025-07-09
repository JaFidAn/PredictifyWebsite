using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class AiForecastConfiguration : IEntityTypeConfiguration<AiForecast>
{
    public void Configure(EntityTypeBuilder<AiForecast> builder)
    {
        builder.ToTable("AiForecasts");

        builder.HasKey(af => af.Id);

        builder.Property(af => af.MatchId)
            .IsRequired();

        builder.Property(af => af.ProbabilityOfCorrectness)
            .IsRequired()
            .HasColumnType("float");

        builder.Property(af => af.IsActuallyCorrect)
            .IsRequired(false);

        builder.Property(af => af.ModelVersion)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(af => af.Forecast)
            .WithMany()
            .HasForeignKey(af => af.ForecastId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}