
using CampusPlacementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementSystem.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.CompanyId);

        builder.Property(c => c.CompanyName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.CompanyCode)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.Industry)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Website)
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.ContactEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.ContactPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Status)
            .HasConversion<int>();

        builder.HasIndex(c => c.CompanyCode)
            .IsUnique();
    }
}
