using Domain;
using Domain.Interfaces;
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
            return await _context.Accounts.FindAsync(accountId);
        }

        public async Task CreateAccountAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public void UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
        }

        public void UpdateAccountsRange(IEnumerable<Account> accounts)
        {
            _context.Accounts.UpdateRange(accounts);
        }

        public async Task DeleteAccountAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }
        }

        public async Task<IEnumerable<Account>> GetAllUserAccountsAsync(string userId)
        {
            return await _context.Accounts.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetAllUserAccountsWithTransactionsAsync(string userId)
        {
            return await _context.Accounts
                .Include(a => a.Transactions)
                .ThenInclude(t => t.Category)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }
    }
}
