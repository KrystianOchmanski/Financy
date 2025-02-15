using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public List<Transaction> Transactions { get; set; } = null!;
    }
}
