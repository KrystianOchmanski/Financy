using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction?> GetTransactionWithAccountAsync(int id);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(Expression<Func<Transaction, bool>> predicate);
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Transaction UpdateTransaction(Transaction transaction);
        void DeleteTransaction(Transaction transaction);
    }
}
