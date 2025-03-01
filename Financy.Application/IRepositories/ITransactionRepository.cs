using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Transaction UpdateTransaction(Transaction transaction);
        void DeleteTransaction(Transaction transaction);
    }
}
