using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Domain;

namespace Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId)
        {
            return await _context.Transactions
                                 .Where(t => t.AccountId == accountId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByCategoryIdAsync(int categoryId)
        {
            return await _context.Transactions
                                 .Where(t => t.CategoryId == categoryId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(DateOnly date)
        {
            return await _context.Transactions
                                 .Where(t => t.Date == date)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _context.Transactions
                                 .Where(t => t.Date >= startDate && t.Date <= endDate)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type)
        {
            return await _context.Transactions
                                 .Where(t => t.Type == type)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAmountRangeAsync(decimal minAmount, decimal maxAmount)
        {
            return await _context.Transactions
                                 .Where(t => t.Amount >= minAmount && t.Amount <= maxAmount)
                                 .ToListAsync();
        }

        public async Task CreateTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
    }
}
