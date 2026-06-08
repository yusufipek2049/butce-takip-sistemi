using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ButceTakipSistemi.Models;

public class BudgetGoal
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Gider kategorisi zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Gider kategorisi zorunludur.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Ay zorunludur.")]
    [Range(1, 12, ErrorMessage = "Ay 1 ile 12 arasında olmalıdır.")]
    public int Month { get; set; }

    [Required(ErrorMessage = "Yıl zorunludur.")]
    [Range(2000, 2100, ErrorMessage = "Yıl 2000 ile 2100 arasında olmalıdır.")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Limit tutarı zorunludur.")]
    [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "Limit tutarı 0'dan büyük olmalıdır.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal LimitAmount { get; set; }

    public bool IsActive { get; set; } = true;

    public Category? Category { get; set; }
}
