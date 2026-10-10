using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Account name is required")]
        public string Name { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialBalance { get; set; }

        [JsonIgnore]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public string UserId { get; set; } = null!;

        [JsonIgnore]
        public User User { get; set; } = null!;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
