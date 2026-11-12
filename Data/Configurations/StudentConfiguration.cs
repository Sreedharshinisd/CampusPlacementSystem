
using CampusPlacementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementSystem.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.StudentId);

        builder.Property(s => s.RegisterNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(s => s.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Degree)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.CGPA)
            .HasPrecision(4, 2);

        builder.Property(s => s.ResumeUrl)
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .HasConversion<int>();

        builder.HasIndex(s => s.UserId)
            .IsUnique();

        builder.HasIndex(s => new
        {
            s.CollegeId,
            s.RegisterNumber
        }).IsUnique();

        builder.HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.College)
            .WithMany()
            .HasForeignKey(s => s.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
