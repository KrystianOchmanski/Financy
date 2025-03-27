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
        var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

        user.RefreshToken = refreshToken;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return null;
        }

        return (token, refreshToken);
    }

    public string? RefreshToken(string refreshToken)
    {
        var principal = _jwtService.ValidateToken(refreshToken);
        if (principal == null)
        {
            return null;
        }

        foreach (var claim in principal.Claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return null;
        }

        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return null;
        }

        return _jwtService.GenerateToken(user.Id, user.Email!);
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

    public async Task LogoutAsync(string refreshToken)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        user.RefreshToken = null;

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
