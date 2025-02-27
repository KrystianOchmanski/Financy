namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId, bool includeTransactions = false);

        Task<IEnumerable<Account>> GetAllUserAccountsAsync(string userId, bool includeTransactions = false);

        Task CreateAccountAsync(Account account);

        void UpdateAccount(Account account);

        Task DeleteAccountAsync(int accountId);
    }
}
