using Financy.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Financy.Application.IServices
{
    public interface IAuthService
    {
        Task<(string Token, string RefreshToken)?> LoginAsync(LoginDTO loginDTO);

        Task<(string Token, string RefreshToken)?> RefreshTokenAsync(string refreshToken);

        Task<IdentityResult> RegisterAsync(RegisterDTO registerDTO);

        Task LogoutAsync(string refreshToken);

        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        Task<string> GetUserName(ClaimsPrincipal claimsPrincipal);
    }
}
