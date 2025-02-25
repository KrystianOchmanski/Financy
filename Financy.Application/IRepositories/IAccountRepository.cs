namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId);

        Task<IEnumerable<Account>> GetAllUserAccountsAsync(string userId);

        Task<IEnumerable<Account>> GetAllUserAccountsWithTransactionsAsync(string userId);

        Task CreateAccountAsync(Account account);

        void UpdateAccount(Account account);

        void UpdateAccountsRange(IEnumerable<Account> accounts);

        Task DeleteAccountAsync(int accountId);
    }
}
