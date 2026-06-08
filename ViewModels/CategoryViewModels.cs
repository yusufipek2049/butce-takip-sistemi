using System.ComponentModel.DataAnnotations;
using ButceTakipSistemi.Models;

namespace ButceTakipSistemi.ViewModels;

public class CategoryInputModel
{
    [Display(Name = "Kategori adı")]
    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [StringLength(80, ErrorMessage = "Kategori adı en fazla 80 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Kategori türü")]
    [Required(ErrorMessage = "Kategori türü zorunludur.")]
    public CategoryType Type { get; set; } = CategoryType.Expense;
}

public class CategoryListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public CategoryType Type { get; set; }

    public bool IsActive { get; set; }

    public bool IsUsed { get; set; }
}
