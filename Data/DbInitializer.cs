using Microsoft.EntityFrameworkCore;
using ButceTakipSistemi.Models;

namespace ButceTakipSistemi.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Categories.AnyAsync())
        {
            await NormalizeExistingSeedTextAsync(context);
            return;
        }

        var categories = new List<Category>
        {
            new() { Name = "Maaş", Type = CategoryType.Income },
            new() { Name = "Freelance", Type = CategoryType.Income },
            new() { Name = "Burs", Type = CategoryType.Income },
            new() { Name = "Market", Type = CategoryType.Expense },
            new() { Name = "Kira", Type = CategoryType.Expense },
            new() { Name = "Ulaşım", Type = CategoryType.Expense },
            new() { Name = "Eğlence", Type = CategoryType.Expense }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        var now = DateTime.Today;
        var market = categories.Single(category => category.Name == "Market");
        var rent = categories.Single(category => category.Name == "Kira");
        var salary = categories.Single(category => category.Name == "Maaş");

        context.BudgetGoals.AddRange(
            new BudgetGoal { CategoryId = market.Id, Month = now.Month, Year = now.Year, LimitAmount = 6500 },
            new BudgetGoal { CategoryId = rent.Id, Month = now.Month, Year = now.Year, LimitAmount = 15000 });

        context.Transactions.AddRange(
            new Transaction
            {
                Type = TransactionType.Income,
                CategoryId = salary.Id,
                Amount = 42000,
                TransactionDate = new DateTime(now.Year, now.Month, 1),
                Description = "Aylık maaş"
            },
            new Transaction
            {
                Type = TransactionType.Expense,
                CategoryId = rent.Id,
                Amount = 15000,
                TransactionDate = new DateTime(now.Year, now.Month, 3),
                Description = "Ev kirasi"
            },
            new Transaction
            {
                Type = TransactionType.Expense,
                CategoryId = market.Id,
                Amount = 4200,
                TransactionDate = new DateTime(now.Year, now.Month, 5),
                Description = "Haftalık market"
            });

        await context.SaveChangesAsync();
    }

    private static async Task NormalizeExistingSeedTextAsync(ApplicationDbContext context)
    {
        var categoryRenames = new Dictionary<string, string>
        {
            ["Maas"] = "Maaş",
            ["Ulasim"] = "Ulaşım",
            ["Eglence"] = "Eğlence"
        };

        var categories = await context.Categories
            .Where(category => categoryRenames.Keys.Contains(category.Name))
            .ToListAsync();

        foreach (var category in categories)
        {
            var newName = categoryRenames[category.Name];
            var targetExists = await context.Categories.AnyAsync(item =>
                item.Id != category.Id &&
                item.Type == category.Type &&
                item.Name == newName);

            if (!targetExists)
            {
                category.Name = newName;
            }
        }

        var descriptionRenames = new Dictionary<string, string>
        {
            ["Aylik maas"] = "Aylık maaş",
            ["Haftalik market"] = "Haftalık market"
        };

        var transactions = await context.Transactions
            .Where(transaction => transaction.Description != null && descriptionRenames.Keys.Contains(transaction.Description))
            .ToListAsync();

        foreach (var transaction in transactions)
        {
            transaction.Description = descriptionRenames[transaction.Description!];
        }

        if (categories.Count > 0 || transactions.Count > 0)
        {
            await context.SaveChangesAsync();
        }
    }
}
