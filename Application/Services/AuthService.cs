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

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return null; // Invalid login data
        }

        return _jwtService.GenerateToken(user.Id, user.Email!);
    }

    public async Task<IdentityResult> RegisterAsync(string email, string password, string firstName)
    {
        var newUser = new User
        {
            FirstName = firstName,
            Email = email,
        };
        return await _userManager.CreateAsync(newUser, password);
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
