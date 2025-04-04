using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(x => x.RevokedAt)
               .IsRequired();

        builder.HasIndex(x => x.Token).IsUnique();
    }
}
