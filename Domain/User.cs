using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = null!;

        public List<Account> Accounts { get; set; } = new List<Account>();

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
