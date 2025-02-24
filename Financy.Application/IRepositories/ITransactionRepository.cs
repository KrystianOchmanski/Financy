namespace Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId);
        Task<IEnumerable<Transaction>> GetTransactionsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(DateOnly date);
        Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type);
        Task<IEnumerable<Transaction>> GetTransactionsByAmountRangeAsync(decimal minAmount, decimal maxAmount);
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Transaction UpdateTransactionAsync(Transaction transaction);
        void DeleteTransaction(Transaction transaction);
    }
}
