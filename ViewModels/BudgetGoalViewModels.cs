using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ButceTakipSistemi.ViewModels;

public class BudgetGoalInputModel
{
    public int? Id { get; set; }

    [Display(Name = "Gider kategorisi")]
    [Required(ErrorMessage = "Gider kategorisi zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Gider kategorisi zorunludur.")]
    public int CategoryId { get; set; }

    [Display(Name = "Ay")]
    [Required(ErrorMessage = "Ay zorunludur.")]
    [Range(1, 12, ErrorMessage = "Ay 1 ile 12 arasında olmalıdır.")]
    public int Month { get; set; } = DateTime.Today.Month;

    [Display(Name = "Yıl")]
    [Required(ErrorMessage = "Yıl zorunludur.")]
    [Range(2000, 2100, ErrorMessage = "Yıl 2000 ile 2100 arasında olmalıdır.")]
    public int Year { get; set; } = DateTime.Today.Year;

    [Display(Name = "Limit tutarı")]
    [Required(ErrorMessage = "Limit tutarı zorunludur.")]
    [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "Limit tutarı 0'dan büyük olmalıdır.")]
    public decimal LimitAmount { get; set; }
}

public class BudgetGoalListItemViewModel
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int Month { get; set; }

    public int Year { get; set; }

    public decimal LimitAmount { get; set; }

    public bool IsActive { get; set; }
}

public class BudgetGoalPageViewModel
{
    public BudgetGoalInputModel Input { get; set; } = new();

    public IReadOnlyList<SelectListItem> ExpenseCategories { get; set; } = [];

    public IReadOnlyList<BudgetGoalListItemViewModel> Goals { get; set; } = [];
}
