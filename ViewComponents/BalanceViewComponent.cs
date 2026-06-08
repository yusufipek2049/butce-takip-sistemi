using Microsoft.AspNetCore.Mvc;
using ButceTakipSistemi.Services;

namespace ButceTakipSistemi.ViewComponents;

public class BalanceViewComponent(IBalanceService balanceService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var balance = await balanceService.GetCurrentBalanceAsync();
        return View(balance);
    }
}
