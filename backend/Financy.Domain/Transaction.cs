using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int AccountId { get; set; }

        [JsonIgnore]
        public Account Account { get; set; } = null!;

        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category Category { get; set; } = null!;
    }
}
