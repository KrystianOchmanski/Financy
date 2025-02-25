using Domain;
using Financy.Application.DTOs.TransactionDTOs;
using System.Linq.Expressions;

namespace Application.IServices
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();

        Task<Transaction?> GetTransactionByIdAsync(int transactionId);

        Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(Expression<Func<Transaction, bool>> predicate);

        Task<Transaction> AddTransactionAsync(CreateTransactionDTO transaction);

        Task<Transaction> UpdateTransactionAsync(EditTransactionDTO transaction);

        Task<bool> DeleteTransactionAsync(int id);
    }
}
