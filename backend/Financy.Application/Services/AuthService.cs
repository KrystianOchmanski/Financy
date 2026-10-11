using Domain;
using Financy.Application.Configuration;
using Financy.Application.DTOs.Auth;
using Financy.Application.IRepositories;
using Financy.Application.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Financy.Application.Services
{
    public class AuthService : IAuthService
    {
        // Parallel refreshes (e.g. two browser tabs) may still send a token that was rotated a moment ago.
        private static readonly TimeSpan RotatedTokenGracePeriod = TimeSpan.FromSeconds(30);

        private readonly UserManager<User> _userManager;
        private readonly JwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<User> userManager,
            JwtService jwtService,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<(string Token, string RefreshToken)?> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                return null; // Invalid login data
            }

            await _refreshTokenRepository.RemoveExpiredAsync(user.Id);
            var refreshToken = await IssueRefreshTokenAsync(user.Id);
            await _unitOfWork.SaveChangesAsync();

            return (_jwtService.GenerateToken(user.Id, user.Email!), refreshToken);
        }

        public async Task<(string Token, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
        {
            var now = DateTime.UtcNow;
            var storedToken = await _refreshTokenRepository.GetByHashAsync(HashRefreshToken(refreshToken));
            if (storedToken == null || storedToken.ExpiresAt <= now)
            {
                return null;
            }

            var wasRotated = storedToken.ReplacedByTokenHash != null;
            if (storedToken.RevokedAt != null && !(wasRotated && now - storedToken.RevokedAt <= RotatedTokenGracePeriod))
            {
                if (wasRotated)
                {
                    // A rotated token used again means it has leaked, so end every session of the user.
                    await RevokeAllRefreshTokensAsync(storedToken.UserId, now);
                    await _unitOfWork.SaveChangesAsync();
                }

                return null;
            }

            var newRefreshToken = await IssueRefreshTokenAsync(storedToken.UserId);
            if (storedToken.RevokedAt == null)
            {
                storedToken.RevokedAt = now;
                storedToken.ReplacedByTokenHash = HashRefreshToken(newRefreshToken);
            }
            await _unitOfWork.SaveChangesAsync();

            return (_jwtService.GenerateToken(storedToken.UserId, storedToken.User.Email!), newRefreshToken);
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
            var storedToken = await _refreshTokenRepository.GetByHashAsync(HashRefreshToken(refreshToken));
            if (storedToken == null || storedToken.RevokedAt != null)
            {
                return;
            }

            storedToken.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
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

        public async Task<string> GetUserName(ClaimsPrincipal claimsPrincipal)
        {
            var userId = _userManager.GetUserId(claimsPrincipal);
            if (userId == null)
                throw new UnauthorizedAccessException("Invalid token");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            return user.FirstName;
        }

        private async Task<string> IssueRefreshTokenAsync(string userId)
        {
            var refreshToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var now = DateTime.UtcNow;

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                TokenHash = HashRefreshToken(refreshToken),
                UserId = userId,
                CreatedAt = now,
                ExpiresAt = now.AddDays(_jwtSettings.RefreshTokenExpiryDays)
            });

            return refreshToken;
        }

        private async Task RevokeAllRefreshTokensAsync(string userId, DateTime revokedAt)
        {
            var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(userId);
            foreach (var token in activeTokens)
            {
                token.RevokedAt = revokedAt;
            }
        }

        // Only hashes are stored, so a database leak does not expose usable refresh tokens.
        private static string HashRefreshToken(string refreshToken)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
        }
    }
}
