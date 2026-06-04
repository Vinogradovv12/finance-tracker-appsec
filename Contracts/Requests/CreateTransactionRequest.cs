using System.ComponentModel.DataAnnotations;
using FinanceTracker.Api.Domain;

namespace FinanceTracker.Api.Contracts.Requests;

public class CreateTransactionRequest
{
    [Range(0.01, 1_000_000)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Type is required")]
    public TransactionsType? Type { get; set; }
}