using Application.DTOs;
using Domain;
using System.Linq.Expressions;

namespace Application.IServices
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();

        Task<Transaction?> GetTransactionByIdAsync(int transactionId);

        Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(Expression<Func<Transaction, bool>> predicate);

        Task<Transaction> AddTransactionAsync(AddTransactionDTO transaction);

        void UpdateTransaction(Transaction transaction);

        Task<bool> DeleteTransactionAsync(int id);
    }
}
