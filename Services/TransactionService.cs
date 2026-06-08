using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ButceTakipSistemi.Data;
using ButceTakipSistemi.Models;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<SelectListItem>> GetCategoryOptionsAsync(TransactionType type);

    Task<IReadOnlyList<TransactionListItemViewModel>> GetTransactionsAsync();

    Task<TransactionInputModel?> GetTransactionForEditAsync(int id);

    Task<(bool Success, string? Error)> SaveAsync(TransactionInputModel input);

    Task<bool> DeleteAsync(int id);

    Task<BudgetStatusViewModel?> GetBudgetWarningAsync(int expenseCategoryId, DateTime transactionDate);
}

public class TransactionService(ApplicationDbContext context) : ITransactionService
{
    public async Task<IReadOnlyList<SelectListItem>> GetCategoryOptionsAsync(TransactionType type)
    {
        var categoryType = ToCategoryType(type);

        return await context.Categories
            .AsNoTracking()
            .Where(category => category.IsActive && category.Type == categoryType)
            .OrderBy(category => category.Name)
            .Select(category => new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = category.Name
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TransactionListItemViewModel>> GetTransactionsAsync()
    {
        return await context.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.Category)
            .OrderByDescending(transaction => transaction.TransactionDate)
            .ThenByDescending(transaction => transaction.Id)
            .Select(transaction => new TransactionListItemViewModel
            {
                Id = transaction.Id,
                Type = transaction.Type,
                CategoryName = transaction.Category!.Name,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Description = transaction.Description
            })
            .ToListAsync();
    }

    public async Task<TransactionInputModel?> GetTransactionForEditAsync(int id)
    {
        return await context.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.Id == id)
            .Select(transaction => new TransactionInputModel
            {
                Id = transaction.Id,
                Type = transaction.Type,
                CategoryId = transaction.CategoryId,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Description = transaction.Description
            })
            .SingleOrDefaultAsync();
    }

    public async Task<(bool Success, string? Error)> SaveAsync(TransactionInputModel input)
    {
        var categoryType = ToCategoryType(input.Type);
        var categoryExists = await context.Categories
            .AnyAsync(category => category.Id == input.CategoryId && category.Type == categoryType && category.IsActive);

        if (!categoryExists)
        {
            return (false, "Seçilen kategori aktif değil veya işlem türü ile uyumlu değil.");
        }

        if (input.Id is null)
        {
            context.Transactions.Add(new Transaction
            {
                Type = input.Type,
                CategoryId = input.CategoryId,
                Amount = input.Amount,
                TransactionDate = input.TransactionDate.Date,
                Description = input.Description
            });
        }
        else
        {
            var transaction = await context.Transactions.FindAsync(input.Id.Value);
            if (transaction is null)
            {
                return (false, "İşlem kaydı bulunamadı.");
            }

            transaction.Type = input.Type;
            transaction.CategoryId = input.CategoryId;
            transaction.Amount = input.Amount;
            transaction.TransactionDate = input.TransactionDate.Date;
            transaction.Description = input.Description;
        }

        await context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var transaction = await context.Transactions.FindAsync(id);
        if (transaction is null)
        {
            return false;
        }

        context.Transactions.Remove(transaction);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<BudgetStatusViewModel?> GetBudgetWarningAsync(int expenseCategoryId, DateTime transactionDate)
    {
        var goal = await context.BudgetGoals
            .AsNoTracking()
            .Include(budgetGoal => budgetGoal.Category)
            .Where(budgetGoal =>
                budgetGoal.IsActive &&
                budgetGoal.CategoryId == expenseCategoryId &&
                budgetGoal.Month == transactionDate.Month &&
                budgetGoal.Year == transactionDate.Year)
            .Select(budgetGoal => new
            {
                budgetGoal.Id,
                CategoryName = budgetGoal.Category!.Name,
                budgetGoal.LimitAmount
            })
            .SingleOrDefaultAsync();

        if (goal is null)
        {
            return null;
        }

        var spentAmount = await context.Transactions
            .AsNoTracking()
            .Where(transaction =>
                transaction.Type == TransactionType.Expense &&
                transaction.CategoryId == expenseCategoryId &&
                transaction.TransactionDate.Month == transactionDate.Month &&
                transaction.TransactionDate.Year == transactionDate.Year)
            .SumAsync(transaction => (double?)transaction.Amount) ?? 0;

        var status = new BudgetStatusViewModel
        {
            BudgetGoalId = goal.Id,
            CategoryName = goal.CategoryName,
            LimitAmount = goal.LimitAmount,
            SpentAmount = (decimal)spentAmount
        };

        return status.UsagePercent >= 80 ? status : null;
    }

    private static CategoryType ToCategoryType(TransactionType type)
    {
        return type == TransactionType.Income ? CategoryType.Income : CategoryType.Expense;
    }
}
