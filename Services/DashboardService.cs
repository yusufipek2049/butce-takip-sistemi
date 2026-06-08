using Microsoft.EntityFrameworkCore;
using ButceTakipSistemi.Data;
using ButceTakipSistemi.Models;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(int month, int year);
}

public class DashboardService(ApplicationDbContext context) : IDashboardService
{
    public async Task<DashboardViewModel> GetDashboardAsync(int month, int year)
    {
        var periodTransactions = context.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.TransactionDate.Month == month && transaction.TransactionDate.Year == year);

        var totalIncome = await periodTransactions
            .Where(transaction => transaction.Type == TransactionType.Income)
            .SumAsync(transaction => (double?)transaction.Amount) ?? 0;

        var totalExpense = await periodTransactions
            .Where(transaction => transaction.Type == TransactionType.Expense)
            .SumAsync(transaction => (double?)transaction.Amount) ?? 0;

        var categoryExpenseRows = await periodTransactions
            .Where(transaction => transaction.Type == TransactionType.Expense)
            .GroupBy(transaction => transaction.Category!.Name)
            .Select(group => new
            {
                CategoryName = group.Key,
                TotalAmount = group.Sum(transaction => (double)transaction.Amount)
            })
            .ToListAsync();

        var spentByCategory = await periodTransactions
            .Where(transaction => transaction.Type == TransactionType.Expense)
            .GroupBy(transaction => transaction.CategoryId)
            .Select(group => new
            {
                CategoryId = group.Key,
                SpentAmount = group.Sum(transaction => (double)transaction.Amount)
            })
            .ToListAsync();

        var budgetGoals = await context.BudgetGoals
            .AsNoTracking()
            .Include(goal => goal.Category)
            .Where(goal => goal.IsActive && goal.Month == month && goal.Year == year)
            .Select(goal => new
            {
                goal.Id,
                goal.CategoryId,
                CategoryName = goal.Category!.Name,
                goal.LimitAmount
            })
            .ToListAsync();

        var budgetStatuses = budgetGoals
            .Select(goal =>
            {
                var spent = spentByCategory.FirstOrDefault(item => item.CategoryId == goal.CategoryId)?.SpentAmount ?? 0;
                return new BudgetStatusViewModel
                {
                    BudgetGoalId = goal.Id,
                    CategoryName = goal.CategoryName,
                    LimitAmount = goal.LimitAmount,
                    SpentAmount = (decimal)spent
                };
            })
            .OrderByDescending(status => status.UsagePercent)
            .ThenBy(status => status.CategoryName)
            .ToList();

        return new DashboardViewModel
        {
            Month = month,
            Year = year,
            TotalIncome = (decimal)totalIncome,
            TotalExpense = (decimal)totalExpense,
            CategoryExpenseSummaries = categoryExpenseRows
                .Select(summary => new CategoryExpenseSummaryViewModel
                {
                    CategoryName = summary.CategoryName,
                    TotalAmount = (decimal)summary.TotalAmount
                })
                .OrderByDescending(summary => summary.TotalAmount)
                .ToList(),
            BudgetStatuses = budgetStatuses
        };
    }
}
