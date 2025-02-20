using Application.DTOs;
using Domain;

namespace Application.IServices
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();

        Task<Transaction?> GetTransactionByIdAsync(int transactionId);

        Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId);

        Task<IEnumerable<Transaction>> GetTransactionsByCategoryIdAsync(int categoryId);

        Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(DateOnly date);

        Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateOnly startDate, DateOnly endDate);

        Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type);

        Task<IEnumerable<Transaction>> GetTransactionsByAmountRangeAsync(decimal minAmount, decimal maxAmount);

        Task<Transaction> AddTransactionAsync(AddTransactionDTO transaction);

        void UpdateTransaction(Transaction transaction);

        Task<bool> DeleteTransactionAsync(int id);
    }
}
