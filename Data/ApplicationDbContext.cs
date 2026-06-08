using Microsoft.EntityFrameworkCore;
using ButceTakipSistemi.Models;

namespace ButceTakipSistemi.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<BudgetGoal> BudgetGoals => Set<BudgetGoal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasIndex(category => new { category.Type, category.Name })
            .IsUnique();

        modelBuilder.Entity<BudgetGoal>()
            .HasIndex(goal => new { goal.CategoryId, goal.Month, goal.Year, goal.IsActive })
            .IsUnique()
            .HasFilter("\"IsActive\" = 1");

        modelBuilder.Entity<Transaction>()
            .Property(transaction => transaction.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<BudgetGoal>()
            .Property(goal => goal.LimitAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .HasOne(transaction => transaction.Category)
            .WithMany(category => category.Transactions)
            .HasForeignKey(transaction => transaction.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BudgetGoal>()
            .HasOne(goal => goal.Category)
            .WithMany(category => category.BudgetGoals)
            .HasForeignKey(goal => goal.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
