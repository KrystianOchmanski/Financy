using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = null!;

        [JsonIgnore]
        public List<Account> Accounts { get; set; } = new List<Account>();

        public string? RefreshToken { get; set; }
    }
}
