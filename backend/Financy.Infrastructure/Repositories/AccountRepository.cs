using Domain;
using Financy.Application.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(int accountId)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId);
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            var newAccount = await _context.Accounts.AddAsync(account);
            return newAccount.Entity;
        }

        public Account UpdateAccount(Account account)
        {
            var updatedAccount = _context.Accounts.Update(account);
            return updatedAccount.Entity;
        }

        public bool DeleteAccount(Account account)
        {
            var result = _context.Accounts.Remove(account);

            if (result.State == EntityState.Deleted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<Account>> GetAllUserAccountsAsync(string userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetAllUserAccountWithLastTransactionsAsync(string userId, int size)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new Account
                {
                    Id = a.Id,
                    Name = a.Name,
                    InitialBalance = a.InitialBalance,
                    UserId = a.UserId,
                    Transactions = a.Transactions
                        .OrderByDescending(t => t.Date)
                        .Take(size)
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<decimal> GetAccountBalanceAsync(int accountId)
        {
            var initialBalance = await _context.Accounts
                .Where(a => a.Id == accountId)
                .Select(a => a.InitialBalance)
                .FirstOrDefaultAsync();

            var transactionBalance = await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .Select(t => (decimal?)((t.Type == TransactionType.Expense ||
                    (t.Type == TransactionType.Transfer && t.TransferDirection == TransferDirection.Outgoing))
                    ? -t.Amount
                    : t.Amount))
                .SumAsync() ?? 0m;

            return initialBalance + transactionBalance;
        }

        public async Task<decimal> GetUserBalanceAsync(string userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => a.InitialBalance +
                    (_context.Transactions
                        .Where(t => t.AccountId == a.Id)
                        .Select(t => (decimal?)((t.Type == TransactionType.Expense ||
                            (t.Type == TransactionType.Transfer && t.TransferDirection == TransferDirection.Outgoing))
                            ? -t.Amount
                            : t.Amount))
                        .Sum() ?? 0m))
                .SumAsync();
        }

        async Task<IEnumerable<Tuple<Account, decimal>>> IAccountRepository.GetUserAccountsWithBalanceAsync(string userId)
        {
            var transactionSums = _context.Transactions
                .GroupBy(t => t.AccountId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    BalanceDelta = g.Sum(t => (t.Type == TransactionType.Expense ||
                        (t.Type == TransactionType.Transfer && t.TransferDirection == TransferDirection.Outgoing))
                        ? -t.Amount
                        : t.Amount)
                });

            var accountsWithBalance = await _context.Accounts
                .Where(a => a.UserId == userId)
                .GroupJoin(
                    transactionSums,
                    account => account.Id,
                    sum => sum.AccountId,
                    (account, sums) => new { account, sums })
                .SelectMany(
                    x => x.sums.DefaultIfEmpty(),
                    (x, sum) => Tuple.Create(x.account, x.account.InitialBalance + (sum != null ? sum.BalanceDelta : 0m)))
                .ToListAsync();

            return accountsWithBalance;
        }

        async Task<List<Tuple<int, decimal>>> IAccountRepository.GetBalanceForUserAccountsAsync(string userId)
        {
            var transactionSums = _context.Transactions
                .GroupBy(t => t.AccountId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    BalanceDelta = g.Sum(t => (t.Type == TransactionType.Expense ||
                        (t.Type == TransactionType.Transfer && t.TransferDirection == TransferDirection.Outgoing))
                        ? -t.Amount
                        : t.Amount)
                });

            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .GroupJoin(
                    transactionSums,
                    account => account.Id,
                    sum => sum.AccountId,
                    (account, sums) => new { account, sums })
                .SelectMany(
                    x => x.sums.DefaultIfEmpty(),
                    (x, sum) => Tuple.Create(x.account.Id, x.account.InitialBalance + (sum != null ? sum.BalanceDelta : 0m)))
                .ToListAsync();
        }
    }
}
