using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Transaction
    { 
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be grater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateOnly Date {  get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;

        [Required]
        public TransactionType Type { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; } = null!;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;
    }
}
