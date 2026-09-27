using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [JsonIgnore]
        public List<Transaction> Transactions { get; set; } = null!;
    }
}
