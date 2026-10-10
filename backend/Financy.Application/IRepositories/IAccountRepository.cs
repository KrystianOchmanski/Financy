using Domain;

namespace Financy.Application.IRepositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId);

        Task<IEnumerable<Account>> GetAllUserAccountsAsync(string userId);
        Task<IEnumerable<Account>> GetAllUserAccountWithLastTransactionsAsync(string userId, int size);

        Task<decimal> GetAccountBalanceAsync(int accountId);

        Task<decimal> GetUserBalanceAsync(string userId);

        Task<Account> CreateAccountAsync(Account account);

        Account UpdateAccount(Account account);

        bool DeleteAccount(Account account);

        Task<IEnumerable<Tuple<Account, decimal>>> GetUserAccountsWithBalanceAsync(string userId);

        Task<List<Tuple<int, decimal>>> GetBalanceForUserAccountsAsync(string userId);
    }
}
