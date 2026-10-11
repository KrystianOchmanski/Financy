using Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Financy.Application.DTOs.TransactionDTOs
{
    public class CreateTransactionDTO
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be grater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateOnly? Date { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;

        [Required]
        public TransactionType Type { get; set; }

        public int AccountId { get; set; }

        public int CategoryId { get; set; }

        public static implicit operator Transaction(CreateTransactionDTO dto)
        {
            return new Transaction
            {
                Amount = dto.Amount,
                Date = dto.Date ?? DateOnly.FromDateTime(DateTime.UtcNow),
                Description = dto.Description,
                Type = dto.Type,
                AccountId = dto.AccountId,
                CategoryId = dto.CategoryId,
            };
        }
    }
}
