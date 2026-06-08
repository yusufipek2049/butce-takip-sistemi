using Microsoft.EntityFrameworkCore;
using ButceTakipSistemi.Data;
using ButceTakipSistemi.Models;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryListItemViewModel>> GetCategoriesAsync();

    Task<(bool Success, string? Error)> CreateAsync(CategoryInputModel input);

    Task<(bool Success, string? Message)> ToggleActiveAsync(int id);
}

public class CategoryService(ApplicationDbContext context) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryListItemViewModel>> GetCategoriesAsync()
    {
        return await context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Type)
            .ThenBy(category => category.Name)
            .Select(category => new CategoryListItemViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                IsActive = category.IsActive,
                IsUsed = category.Transactions.Any() || category.BudgetGoals.Any()
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error)> CreateAsync(CategoryInputModel input)
    {
        var normalizedName = input.Name.Trim();
        var duplicateExists = await context.Categories
            .AnyAsync(category => category.Type == input.Type && category.Name.ToLower() == normalizedName.ToLower());

        if (duplicateExists)
        {
            return (false, "Aynı türde aynı kategori adı tekrar eklenemez.");
        }

        context.Categories.Add(new Category
        {
            Name = normalizedName,
            Type = input.Type,
            IsActive = true
        });

        await context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Message)> ToggleActiveAsync(int id)
    {
        var category = await context.Categories
            .Include(item => item.Transactions)
            .Include(item => item.BudgetGoals)
            .SingleOrDefaultAsync(item => item.Id == id);

        if (category is null)
        {
            return (false, "Kategori bulunamadı.");
        }

        if (category.IsActive)
        {
            category.IsActive = false;
            await context.SaveChangesAsync();
            return (true, category.Transactions.Any() || category.BudgetGoals.Any()
                ? "Kategori kullanıldığı için fiziksel silinmedi, pasifleştirildi."
                : "Kategori pasifleştirildi.");
        }

        category.IsActive = true;
        await context.SaveChangesAsync();
        return (true, "Kategori yeniden aktif hale getirildi.");
    }
}
