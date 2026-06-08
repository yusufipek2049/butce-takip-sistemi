using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ButceTakipSistemi.Services;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Pages.Transactions;

public class DeleteModel(ITransactionService transactionService) : PageModel
{
    public TransactionListItemViewModel? Transaction { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Transaction = (await transactionService.GetTransactionsAsync())
            .SingleOrDefault(transaction => transaction.Id == id);

        return Transaction is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var deleted = await transactionService.DeleteAsync(id);
        TempData["SuccessMessage"] = deleted ? "İşlem kaydı silindi." : "İşlem kaydı bulunamadı.";
        return RedirectToPage("/Transactions/Index");
    }
}
