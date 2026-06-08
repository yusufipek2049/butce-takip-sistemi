using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ButceTakipSistemi.Data;
using ButceTakipSistemi.Models;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Services;

public interface IBudgetGoalService
{
    Task<BudgetGoalPageViewModel> GetPageViewModelAsync(BudgetGoalInputModel? input = null);

    Task<BudgetGoalInputModel?> GetForEditAsync(int id);

    Task<(bool Success, string? Error, bool UpdatedExisting)> SaveAsync(BudgetGoalInputModel input);

    Task<bool> DeactivateAsync(int id);
}

public class BudgetGoalService(ApplicationDbContext context) : IBudgetGoalService
{
    public async Task<BudgetGoalPageViewModel> GetPageViewModelAsync(BudgetGoalInputModel? input = null)
    {
        var categories = await context.Categories
            .AsNoTracking()
            .Where(category => category.IsActive && category.Type == CategoryType.Expense)
            .OrderBy(category => category.Name)
            .Select(category => new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = category.Name
            })
            .ToListAsync();

        var goals = await context.BudgetGoals
            .AsNoTracking()
            .Include(goal => goal.Category)
            .OrderByDescending(goal => goal.Year)
            .ThenByDescending(goal => goal.Month)
            .ThenBy(goal => goal.Category!.Name)
            .Select(goal => new BudgetGoalListItemViewModel
            {
                Id = goal.Id,
                CategoryName = goal.Category!.Name,
                Month = goal.Month,
                Year = goal.Year,
                LimitAmount = goal.LimitAmount,
                IsActive = goal.IsActive
            })
            .ToListAsync();

        return new BudgetGoalPageViewModel
        {
            Input = input ?? new BudgetGoalInputModel(),
            ExpenseCategories = categories,
            Goals = goals
        };
    }

    public async Task<BudgetGoalInputModel?> GetForEditAsync(int id)
    {
        return await context.BudgetGoals
            .AsNoTracking()
            .Where(goal => goal.Id == id)
            .Select(goal => new BudgetGoalInputModel
            {
                Id = goal.Id,
                CategoryId = goal.CategoryId,
                Month = goal.Month,
                Year = goal.Year,
                LimitAmount = goal.LimitAmount
            })
            .SingleOrDefaultAsync();
    }

    public async Task<(bool Success, string? Error, bool UpdatedExisting)> SaveAsync(BudgetGoalInputModel input)
    {
        var categoryIsExpense = await context.Categories
            .AnyAsync(category => category.Id == input.CategoryId && category.Type == CategoryType.Expense && category.IsActive);

        if (!categoryIsExpense)
        {
            return (false, "Bütçe hedefi yalnızca aktif gider kategorisi için tanımlanabilir.", false);
        }

        if (input.Id is not null)
        {
            var editableGoal = await context.BudgetGoals.FindAsync(input.Id.Value);
            if (editableGoal is null)
            {
                return (false, "Bütçe hedefi bulunamadı.", false);
            }

            var duplicateForEdit = await context.BudgetGoals.AnyAsync(goal =>
                goal.Id != input.Id.Value &&
                goal.IsActive &&
                goal.CategoryId == input.CategoryId &&
                goal.Month == input.Month &&
                goal.Year == input.Year);

            if (duplicateForEdit)
            {
                return (false, "Aynı kategori-ay-yıl için aktif bir bütçe hedefi zaten var.", false);
            }

            editableGoal.CategoryId = input.CategoryId;
            editableGoal.Month = input.Month;
            editableGoal.Year = input.Year;
            editableGoal.LimitAmount = input.LimitAmount;
            editableGoal.IsActive = true;
            await context.SaveChangesAsync();
            return (true, null, false);
        }

        var existingGoal = await context.BudgetGoals.SingleOrDefaultAsync(goal =>
            goal.IsActive &&
            goal.CategoryId == input.CategoryId &&
            goal.Month == input.Month &&
            goal.Year == input.Year);

        if (existingGoal is not null)
        {
            existingGoal.LimitAmount = input.LimitAmount;
            await context.SaveChangesAsync();
            return (true, null, true);
        }

        context.BudgetGoals.Add(new BudgetGoal
        {
            CategoryId = input.CategoryId,
            Month = input.Month,
            Year = input.Year,
            LimitAmount = input.LimitAmount,
            IsActive = true
        });

        await context.SaveChangesAsync();
        return (true, null, false);
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var goal = await context.BudgetGoals.FindAsync(id);
        if (goal is null)
        {
            return false;
        }

        goal.IsActive = false;
        await context.SaveChangesAsync();
        return true;
    }
}
