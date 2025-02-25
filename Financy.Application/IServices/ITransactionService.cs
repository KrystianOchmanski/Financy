using Domain;
using Financy.Application.DTOs.TransactionDTOs;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Application.IServices
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllUserTransactionsAsync(ClaimsPrincipal userClaims);

        Task<Transaction?> GetTransactionByIdAsync(ClaimsPrincipal userClaims, int transactionId);

        Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(ClaimsPrincipal userClaims, Expression<Func<Transaction, bool>> predicate);

        Task<Transaction> AddTransactionAsync(ClaimsPrincipal userClaims, CreateTransactionDTO transactionDto);

        Task<Transaction> UpdateTransactionAsync(ClaimsPrincipal userClaims, EditTransactionDTO editedTransaction);

        Task DeleteTransactionAsync(ClaimsPrincipal userClaims, int id);
    }
}
