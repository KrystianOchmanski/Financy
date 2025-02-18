using Application.DTOs;
using Domain;
using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtService _jwtService;

    public AuthService(UserManager<User> userManager, SignInManager<User> signInManager,
                                JwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<string?> LoginAsync(LoginDTO loginDTO)
    {
        var user = await _userManager.FindByEmailAsync(loginDTO.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
        {
            return null; // Invalid login data
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

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
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
