namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId);

        Task<IEnumerable<Account>> GetAllAccountsByUserIdAsync(string userId);

        Task CreateAccountAsync(Account account);

        void UpdateAccount(Account account);

        void UpdateAccountsRange(IEnumerable<Account> accounts);

        Task DeleteAccountAsync(int accountId);
    }
}
