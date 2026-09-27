using System.Drawing;

namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId, bool includeTransactions = false);

        Task<IEnumerable<Account>> GetAllUserAccountsAsync(string userId, bool includeTransactions = false);
        Task<IEnumerable<Account>> GetAllUserAccountWithLastTransactionsAsync(string userId, int size);

        Task<Account> CreateAccountAsync(Account account);

        Account UpdateAccount(Account account);

        bool DeleteAccount(Account account);
    }
}
