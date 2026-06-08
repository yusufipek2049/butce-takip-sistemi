using Microsoft.EntityFrameworkCore;
using ButceTakipSistemi.Data;
using ButceTakipSistemi.Models;

namespace ButceTakipSistemi.Services;

public interface IBalanceService
{
    Task<decimal> GetCurrentBalanceAsync();
}

public class BalanceService(ApplicationDbContext context) : IBalanceService
{
    public async Task<decimal> GetCurrentBalanceAsync()
    {
        var income = await context.Transactions
            .Where(transaction => transaction.Type == TransactionType.Income)
            .SumAsync(transaction => (double?)transaction.Amount) ?? 0;

        var expense = await context.Transactions
            .Where(transaction => transaction.Type == TransactionType.Expense)
            .SumAsync(transaction => (double?)transaction.Amount) ?? 0;

        return (decimal)(income - expense);
    }
}
