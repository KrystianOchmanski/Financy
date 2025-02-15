using Application.DTOs;
using Microsoft.AspNetCore.Identity;

public interface IAuthService
{
    Task<string?> LoginAsync(string email, string password);
    Task<IdentityResult> RegisterAsync(RegisterDTO registerDTO);
    Task LogoutAsync();
    Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
