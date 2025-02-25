using Domain;
using Financy.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public interface IAuthService
{
    Task<(string Token, string RefreshToken)?> LoginAsync(LoginDTO loginDTO);

    string? RefreshToken(string refreshToken);

    Task<IdentityResult> RegisterAsync(RegisterDTO registerDTO);

    Task LogoutAsync(ClaimsPrincipal userClaims);

    Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
