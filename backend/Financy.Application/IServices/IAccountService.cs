using Financy.Application.DTOs.AccountDTOs;
using System.Security.Claims;

namespace Financy.Application.IServices
{
    public interface IAccountService
    {
        Task<AccountDTO?> GetByIdAsync(ClaimsPrincipal userClaims, int id);

        Task<List<AccountDTO>> GetUserAccountsAsync(ClaimsPrincipal userClaims, bool includeTransactions);

        Task<AccountDTO> AddUserAccountAsync(ClaimsPrincipal userClaims, CreateAccountDTO accountDTO);

        Task<bool> DeleteAccount(ClaimsPrincipal userClaims, int id);
    }
}
