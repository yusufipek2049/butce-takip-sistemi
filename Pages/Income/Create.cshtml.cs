using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ButceTakipSistemi.Models;
using ButceTakipSistemi.Services;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Pages.Income;

public class CreateModel(ITransactionService transactionService) : PageModel
{
    [BindProperty]
    public TransactionInputModel Input { get; set; } = new()
    {
        Type = TransactionType.Income,
        TransactionDate = DateTime.Today
    };

    public TransactionFormViewModel Form { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadFormAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Type = TransactionType.Income;

        if (!ModelState.IsValid)
        {
            await LoadFormAsync();
            return Page();
        }

        var result = await transactionService.SaveAsync(Input);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Gelir kaydı eklenemedi.");
            await LoadFormAsync();
            return Page();
        }

        TempData["SuccessMessage"] = "Gelir kaydı eklendi.";
        return RedirectToPage("/Index", new { month = Input.TransactionDate.Month, year = Input.TransactionDate.Year });
    }

    private async Task LoadFormAsync()
    {
        Form = new TransactionFormViewModel
        {
            Input = Input,
            Categories = await transactionService.GetCategoryOptionsAsync(TransactionType.Income)
        };
    }
}
