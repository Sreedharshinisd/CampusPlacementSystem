
using CampusPlacementSystem.Models;
using CampusPlacementSystem.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementSystem.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<College> Colleges { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(rt => rt.UserId)
                .IsRequired();

            entity.HasIndex(rt => rt.TokenHash)
                .IsUnique();

            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.ApplyConfiguration(new CollegeConfiguration());
        builder.ApplyConfiguration(new StudentConfiguration());
    }
}
