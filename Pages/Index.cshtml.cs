using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ButceTakipSistemi.Services;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Pages;

public class IndexModel(IDashboardService dashboardService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? Month { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Year { get; set; }

    public DashboardViewModel Dashboard { get; set; } = new();

    public async Task OnGetAsync()
    {
        var today = DateTime.Today;
        var selectedMonth = Month is >= 1 and <= 12 ? Month.Value : today.Month;
        var selectedYear = Year is >= 2000 and <= 2100 ? Year.Value : today.Year;

        Month = selectedMonth;
        Year = selectedYear;
        Dashboard = await dashboardService.GetDashboardAsync(selectedMonth, selectedYear);
    }
}
