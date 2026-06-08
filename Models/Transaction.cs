using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ButceTakipSistemi.Models;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Kategori seçimi zorunludur.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Tutar zorunludur.")]
    [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Tarih zorunludur.")]
    [DataType(DataType.Date)]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
    public string? Description { get; set; }

    public Category? Category { get; set; }
}
