using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string TokenHash { get; set; } = null!;

        public string UserId { get; set; } = null!;

        [JsonIgnore]
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        [MaxLength(64)]
        public string? ReplacedByTokenHash { get; set; }
    }
}
