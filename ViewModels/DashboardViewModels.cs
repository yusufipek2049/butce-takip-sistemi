namespace ButceTakipSistemi.ViewModels;

public class DashboardViewModel
{
    public int Month { get; set; }

    public int Year { get; set; }

    public decimal TotalIncome { get; set; }

    public decimal TotalExpense { get; set; }

    public decimal NetBalance => TotalIncome - TotalExpense;

    public string DebtStatusTitle
    {
        get
        {
            if (NetBalance >= 0)
            {
                return "Rahat bölge";
            }

            if (TotalIncome > 0 && Math.Abs(NetBalance) <= TotalIncome * 0.25m)
            {
                return "Dikkat bölgesi";
            }

            return "Borç riski yüksek";
        }
    }

    public string DebtStatusMessage
    {
        get
        {
            if (NetBalance >= 0)
            {
                return "Bu ay gelirlerin giderlerini karşılıyor. Planın güvenli tarafta.";
            }

            if (TotalIncome > 0 && Math.Abs(NetBalance) <= TotalIncome * 0.25m)
            {
                return "Giderler gelirleri biraz geçmiş. Bir sonraki harcamada limiti kontrol etmek iyi olur.";
            }

            return "Giderler gelirlerin belirgin üstünde. Önce zorunlu olmayan harcamaları gözden geçir.";
        }
    }

    public string DebtStatusCssClass
    {
        get
        {
            if (NetBalance >= 0)
            {
                return "debt-safe";
            }

            if (TotalIncome > 0 && Math.Abs(NetBalance) <= TotalIncome * 0.25m)
            {
                return "debt-watch";
            }

            return "debt-risk";
        }
    }

    public IReadOnlyList<CategoryExpenseSummaryViewModel> CategoryExpenseSummaries { get; set; } = [];

    public IReadOnlyList<BudgetStatusViewModel> BudgetStatuses { get; set; } = [];
}

public class CategoryExpenseSummaryViewModel
{
    public string CategoryName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
}

public class BudgetStatusViewModel
{
    public int BudgetGoalId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public decimal LimitAmount { get; set; }

    public decimal SpentAmount { get; set; }

    public decimal RemainingAmount => LimitAmount - SpentAmount;

    public decimal UsagePercent => LimitAmount <= 0 ? 0 : Math.Round(SpentAmount / LimitAmount * 100, 1);

    public string Status
    {
        get
        {
            if (SpentAmount > LimitAmount)
            {
                return "Limit aşıldı";
            }

            if (UsagePercent >= 80)
            {
                return "Limite yaklaşıldı";
            }

            return "Kontrol altında";
        }
    }

    public string AlertCssClass
    {
        get
        {
            if (SpentAmount > LimitAmount)
            {
                return "danger";
            }

            if (UsagePercent >= 80)
            {
                return "warning";
            }

            return "success";
        }
    }
}
