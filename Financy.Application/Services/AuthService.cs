using Domain;
using Financy.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtService _jwtService;

    public AuthService(UserManager<User> userManager, JwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<(string Token, string RefreshToken)?> LoginAsync(LoginDTO loginDTO)
    {
        var user = await _userManager.FindByEmailAsync(loginDTO.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
        {
            return null; // Invalid login data
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email!);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return null;
        }

        return (token, refreshToken);
    }

    public string? RefreshToken(string refreshToken)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return null;
        }

        var newToken = _jwtService.GenerateToken(user.Id, user.Email!);
        return newToken;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDTO registerDTO)
    {
        var newUser = new User
        {
            FirstName = registerDTO.FirstName,
            Email = registerDTO.Email,
            UserName = registerDTO.Email
        };
        return await _userManager.CreateAsync(newUser, registerDTO.Password);
    }

    public async Task LogoutAsync(ClaimsPrincipal userClaims)
    {
        var userId = _userManager.GetUserId(userClaims);
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new Exception("Invalid token");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("User update went wrong");
        }
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        }

        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }
}
