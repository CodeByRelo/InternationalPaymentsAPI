using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<LoginAudit> LoginAudits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // RELATIONSHIPS

        // USER → PAYMENTS (Customer)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Customer)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // USER → PAYMENTS (Employee verification)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.VerifiedByEmployee)
            .WithMany()
            .HasForeignKey(p => p.VerifiedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // USER → REFRESH TOKENS
        modelBuilder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId);

        // USER → LOGIN AUDITS
        modelBuilder.Entity<LoginAudit>()
            .HasOne<User>()
            .WithMany(u => u.LoginAudits)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}