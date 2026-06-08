using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ButceTakipSistemi.Models;
using ButceTakipSistemi.Services;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Pages.Expenses;

public class CreateModel(ITransactionService transactionService) : PageModel
{
    [BindProperty]
    public TransactionInputModel Input { get; set; } = new()
    {
        Type = TransactionType.Expense,
        TransactionDate = DateTime.Today
    };

    public TransactionFormViewModel Form { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadFormAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Type = TransactionType.Expense;

        if (!ModelState.IsValid)
        {
            await LoadFormAsync();
            return Page();
        }

        var result = await transactionService.SaveAsync(Input);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Gider kaydı eklenemedi.");
            await LoadFormAsync();
            return Page();
        }

        var warning = await transactionService.GetBudgetWarningAsync(Input.CategoryId, Input.TransactionDate);
        TempData["SuccessMessage"] = "Gider kaydı eklendi.";

        if (warning is not null)
        {
            TempData["WarningMessage"] = warning.SpentAmount > warning.LimitAmount
                ? $"{warning.CategoryName} kategorisinde bütçe limiti aşıldı. Harcama: {warning.SpentAmount:N2} TL, limit: {warning.LimitAmount:N2} TL."
                : $"{warning.CategoryName} kategorisi bütçe limitine yaklaşıyor. Kullanım: {warning.UsagePercent:N1}%.";
        }

        return RedirectToPage("/Index", new { month = Input.TransactionDate.Month, year = Input.TransactionDate.Year });
    }

    private async Task LoadFormAsync()
    {
        Form = new TransactionFormViewModel
        {
            Input = Input,
            Categories = await transactionService.GetCategoryOptionsAsync(TransactionType.Expense)
        };
    }
}
