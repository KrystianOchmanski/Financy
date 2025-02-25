using Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Financy.Application.DTOs.TransactionDTOs
{
    public class EditTransactionDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be grater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Date { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToShortDateString();

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;

        public int CategoryId { get; set; }

        public static implicit operator Transaction(EditTransactionDTO dto)
        {
            return new Transaction
            {
                Id = dto.Id,
                Amount = dto.Amount,
                Date = DateOnly.Parse(dto.Date),
                Description = dto.Description,
                CategoryId = dto.CategoryId,
            };
        }
    }
}
