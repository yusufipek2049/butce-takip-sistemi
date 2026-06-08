using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ButceTakipSistemi.Models;
using ButceTakipSistemi.Services;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Pages.Transactions;

public class EditModel(ITransactionService transactionService) : PageModel
{
    [BindProperty]
    public TransactionInputModel Input { get; set; } = new();

    public TransactionFormViewModel Form { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var transaction = await transactionService.GetTransactionForEditAsync(id);
        if (transaction is null)
        {
            return NotFound();
        }

        Input = transaction;
        await LoadFormAsync(Input.Type);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadFormAsync(Input.Type);
            return Page();
        }

        var result = await transactionService.SaveAsync(Input);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "İşlem kaydı güncellenemedi.");
            await LoadFormAsync(Input.Type);
            return Page();
        }

        TempData["SuccessMessage"] = "İşlem kaydı güncellendi.";
        return RedirectToPage("/Transactions/Index");
    }

    private async Task LoadFormAsync(TransactionType type)
    {
        Form = new TransactionFormViewModel
        {
            Input = Input,
            Categories = await transactionService.GetCategoryOptionsAsync(type)
        };
    }
}
