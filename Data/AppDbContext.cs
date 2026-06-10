using Microsoft.EntityFrameworkCore;
using FinanceTracker.Api.Data.Entities;

namespace FinanceTracker.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Unique_User_Email");

        modelBuilder.Entity<TransactionEntity>()
            .HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<TransactionEntity>()
            .HasIndex(t => t.UserId);

        modelBuilder.Entity<TransactionEntity>()
            .ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Transaction_Amount",
                    "\"Amount\" > 0"
                ));
    }
}