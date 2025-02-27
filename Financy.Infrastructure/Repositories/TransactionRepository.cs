using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Domain;
using System.Linq.Expressions;

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
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            var newTransaction = await _context.Transactions.AddAsync(transaction);
            return newTransaction.Entity;
        }

        public Transaction UpdateTransaction(Transaction transaction)
        {
            var updatedTransaction = _context.Transactions.Update(transaction);
            return updatedTransaction.Entity;
        }

        public void DeleteTransaction(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
        }

        public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(Expression<Func<Transaction, bool>> predicate)
        {
            return await _context.Transactions.Where(predicate).ToListAsync();
        }
    }
}
