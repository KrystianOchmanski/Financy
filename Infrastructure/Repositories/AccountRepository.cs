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

        public async Task<IEnumerable<Account>> GetAllAccountsByUserIdAsync(string userId)
        {
            return await _context.Accounts.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task CreateAccountAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccountBalanceAsync(int accountId, decimal newBalance)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                account.Balance = newBalance;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteAccountAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }
    }
}
