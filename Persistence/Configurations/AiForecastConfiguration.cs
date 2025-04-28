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

        builder.Property(af => af.PredictedOutcomeName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(af => af.Confidence)
            .IsRequired()
            .HasColumnType("float");
        
        builder.Property(x => x.IsCorrect)
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