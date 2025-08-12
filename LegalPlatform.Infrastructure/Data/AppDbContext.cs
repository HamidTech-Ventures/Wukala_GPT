using LegalPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LegalPlatform.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<LawyerProfile> LawyerProfiles => Set<LawyerProfile>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<ChatHistory> ChatHistories => Set<ChatHistory>();
    public DbSet<EmailOtp> EmailOtps => Set<EmailOtp>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.HasOne(u => u.LawyerProfile)
                .WithOne(lp => lp.User)
                .HasForeignKey<LawyerProfile>(lp => lp.UserId);
        });

        modelBuilder.Entity<LawyerProfile>(entity =>
        {
            entity.HasIndex(lp => lp.UserId).IsUnique();
            entity.Property(lp => lp.FullName).IsRequired().HasMaxLength(200);
            entity.Property(lp => lp.CNIC).HasMaxLength(50);
            entity.Property(lp => lp.Specialization).HasMaxLength(200);
            entity.Property(lp => lp.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.UserId);
            entity.Property(d => d.OriginalFileName).IsRequired().HasMaxLength(300);
            entity.Property(d => d.ContentType).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<ChatHistory>(entity =>
        {
            entity.HasOne(ch => ch.User)
                .WithMany(u => u.ChatHistories)
                .HasForeignKey(ch => ch.UserId);
        });

        modelBuilder.Entity<EmailOtp>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
        });
    }
}