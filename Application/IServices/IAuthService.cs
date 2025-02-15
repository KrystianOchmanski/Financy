using Domain;
using Microsoft.AspNetCore.Identity;

public interface IAuthService
{
    Task<string?> LoginAsync(string email, string password);
    Task<IdentityResult> RegisterAsync(string email, string password, string firstName);
    Task LogoutAsync();
    Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
