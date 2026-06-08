using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ButceTakipSistemi.Services;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Pages.Budgets;

public class IndexModel(IBudgetGoalService budgetGoalService) : PageModel
{
    [BindProperty]
    public BudgetGoalInputModel Input { get; set; } = new();

    public BudgetGoalPageViewModel PageData { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? editId)
    {
        if (editId is not null)
        {
            var goal = await budgetGoalService.GetForEditAsync(editId.Value);
            if (goal is null)
            {
                return NotFound();
            }

            Input = goal;
        }

        await LoadAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var result = await budgetGoalService.SaveAsync(Input);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Bütçe hedefi kaydedilemedi.");
            await LoadAsync();
            return Page();
        }

        TempData["SuccessMessage"] = result.UpdatedExisting
            ? "Aynı kategori-ay-yıl için mevcut bütçe hedefi güncellendi."
            : "Bütçe hedefi kaydedildi.";

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeactivateAsync(int id)
    {
        var deactivated = await budgetGoalService.DeactivateAsync(id);
        TempData[deactivated ? "SuccessMessage" : "WarningMessage"] = deactivated
            ? "Bütçe hedefi pasifleştirildi."
            : "Bütçe hedefi bulunamadı.";
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        PageData = await budgetGoalService.GetPageViewModelAsync(Input);
    }
}
