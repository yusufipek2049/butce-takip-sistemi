using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ButceTakipSistemi.Models;

namespace ButceTakipSistemi.ViewModels;

public class TransactionInputModel
{
    public int? Id { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [Display(Name = "Kategori")]
    [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Kategori seçimi zorunludur.")]
    public int CategoryId { get; set; }

    [Display(Name = "Tutar")]
    [Required(ErrorMessage = "Tutar zorunludur.")]
    [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
    public decimal Amount { get; set; }

    [Display(Name = "Tarih")]
    [Required(ErrorMessage = "Tarih zorunludur.")]
    [DataType(DataType.Date)]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    [Display(Name = "Açıklama")]
    [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
    public string? Description { get; set; }
}

public class TransactionListItemViewModel
{
    public int Id { get; set; }

    public TransactionType Type { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? Description { get; set; }
}

public class TransactionFormViewModel
{
    public TransactionInputModel Input { get; set; } = new();

    public IReadOnlyList<SelectListItem> Categories { get; set; } = [];
}
