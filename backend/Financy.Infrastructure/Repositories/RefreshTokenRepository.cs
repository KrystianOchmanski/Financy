using Domain;
using Financy.Application.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }

        public async Task<List<RefreshToken>> GetActiveByUserIdAsync(string userId)
        {
            var now = DateTime.UtcNow;

            return await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > now)
                .ToListAsync();
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task RemoveExpiredAsync(string userId)
        {
            var now = DateTime.UtcNow;

            var expiredTokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.ExpiresAt <= now)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(expiredTokens);
        }
    }
}
