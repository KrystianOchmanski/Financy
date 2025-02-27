using Domain;
using Financy.Application.DTOs.TransactionDTOs;
using System.Security.Claims;

namespace Application.IServices
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDTO>> GetAllUserTransactionsAsync(ClaimsPrincipal userClaims);

        Task<TransactionDTO?> GetTransactionByIdAsync(ClaimsPrincipal userClaims, int transactionId);

        Task<IEnumerable<TransactionDTO>> GetFilteredTransactionsAsync(ClaimsPrincipal userClaims, TransactionFilterDTO filter);

        Task<TransactionDTO> AddTransactionAsync(ClaimsPrincipal userClaims, CreateTransactionDTO transactionDto);

        Task<TransactionDTO> UpdateTransactionAsync(ClaimsPrincipal userClaims, EditTransactionDTO editedTransaction);

        Task DeleteTransactionAsync(ClaimsPrincipal userClaims, int id);
    }
}
