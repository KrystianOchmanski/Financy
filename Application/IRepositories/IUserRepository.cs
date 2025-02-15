namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string userId);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string userId);
    }
}
