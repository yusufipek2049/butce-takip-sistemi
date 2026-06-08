using System.ComponentModel.DataAnnotations;

namespace ButceTakipSistemi.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [StringLength(80, ErrorMessage = "Kategori adı en fazla 80 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kategori türü zorunludur.")]
    public CategoryType Type { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public ICollection<BudgetGoal> BudgetGoals { get; set; } = new List<BudgetGoal>();
}
