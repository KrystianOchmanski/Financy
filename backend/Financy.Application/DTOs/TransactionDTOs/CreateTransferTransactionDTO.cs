using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financy.Application.DTOs.TransactionDTOs
{
    public class CreateTransferTransactionDTO
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be grater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SourceAccountId must be greater than 0")]
        public int SourceAccountId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "DestinationAccountId must be greater than 0")]
        public int DestinationAccountId { get; set; }

        public DateOnly? Date { get; set; }
    }
}