using Microsoft.AspNetCore.Mvc.RazorPages;
using ButceTakipSistemi.Services;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Pages.Transactions;

public class IndexModel(ITransactionService transactionService) : PageModel
{
    public IReadOnlyList<TransactionListItemViewModel> Transactions { get; set; } = [];

    public async Task OnGetAsync()
    {
        Transactions = await transactionService.GetTransactionsAsync();
    }
}
