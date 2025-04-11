using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.CountryId)
               .IsRequired();

        builder.HasOne(x => x.Country)
               .WithMany(x => x.Teams)
               .HasForeignKey(x => x.CountryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
