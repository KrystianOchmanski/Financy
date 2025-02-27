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

        public async Task<Account?> GetByIdAsync(int accountId, bool includeTransactions = false)
        {
            return await BuildIncludeQuery(includeTransactions)
                .FirstOrDefaultAsync(a => a.Id == accountId);
        }

        public async Task CreateAccountAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public void UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
        }

        public async Task DeleteAccountAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }
        }

        public async Task<IEnumerable<Account>> GetAllUserAccountsAsync(string userId, bool includeTransactions = false)
        {
            return await BuildIncludeQuery(includeTransactions)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

		/// <summary>
		/// Helper method to build query with optional include
		/// </summary>
		private IQueryable<Account> BuildIncludeQuery(bool includeTransactions)
		{
			var query = _context.Accounts.AsQueryable();

			if (includeTransactions)
			{
				query = query
					.Include(a => a.Transactions)
					.ThenInclude(t => t.Category);
			}

			return query;
		}
	}
}
