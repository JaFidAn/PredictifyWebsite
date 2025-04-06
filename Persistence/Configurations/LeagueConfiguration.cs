using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class LeagueConfiguration : IEntityTypeConfiguration<League>
{
    public void Configure(EntityTypeBuilder<League> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.CountryId)
               .IsRequired();

        builder.Property(x => x.CompetitionId)
               .IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasOne(x => x.Country)
               .WithMany()
               .HasForeignKey(x => x.CountryId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Competition)
               .WithMany(x => x.Leagues)
               .HasForeignKey(x => x.CompetitionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
