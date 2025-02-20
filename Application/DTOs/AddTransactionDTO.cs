using Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AddTransactionDTO
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be grater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Date { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToShortDateString();

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;

        [Required]
        public TransactionType Type { get; set; }

        public int AccountId { get; set; }

        public int CategoryId { get; set; }

        public static implicit operator Transaction(AddTransactionDTO dto)
        {
            return new Transaction
            {
                Amount = dto.Amount,
                Date = DateOnly.Parse(dto.Date),
                Description = dto.Description,
                Type = dto.Type,
                AccountId = dto.AccountId,
                CategoryId = dto.CategoryId,
            };
        }
    }
}
