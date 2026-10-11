using Domain;

namespace Financy.Application.IRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByHashAsync(string tokenHash);
        Task<List<RefreshToken>> GetActiveByUserIdAsync(string userId);
        Task AddAsync(RefreshToken refreshToken);
        Task RemoveExpiredAsync(string userId);
    }
}
