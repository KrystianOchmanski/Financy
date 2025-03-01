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
            
            if(result.State == EntityState.Deleted)
            {
                return true;
            } 
            else
            {
                return false;
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
