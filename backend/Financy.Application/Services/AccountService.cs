using Domain;
using Financy.Application.DTOs.AccountDTOs;
using Financy.Application.Exceptions;
using Financy.Application.IRepositories;
using Financy.Application.IServices;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Financy.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IAccountRepository accountRepository, UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<AccountDTO> AddUserAccountAsync(ClaimsPrincipal userClaims, CreateAccountDTO accountDTO)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid token. Unable to find user.");

            Account newAccount = accountDTO;
            newAccount.User = user;

            var createdAccount = await _accountRepository.CreateAccountAsync(newAccount);
            await _unitOfWork.SaveChangesAsync();
            return createdAccount;
        }

        public async Task<bool> DeleteAccount(ClaimsPrincipal userClaims, int id)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
                throw new KeyNotFoundException($"Account with ID:{id} was not found");

            if (account.UserId != userId)
                throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to account (ID:{account.Id})");

            var result = _accountRepository.DeleteAccount(account);

            if (result)
                await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<AccountDTO?> GetByIdAsync(ClaimsPrincipal userClaims, int id)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var account = await _accountRepository.GetByIdAsync(id);

            if (account == null)
                throw new KeyNotFoundException($"Account ID:{id} was not found.");

            if (account.UserId != userId)
                throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to account ID:{id}.");

            var balance = await _accountRepository.GetAccountBalanceAsync(id);
            return new AccountDTO(account, balance);
        }

        public async Task<List<AccountDTO>> GetUserAccountsAsync(ClaimsPrincipal userClaims)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var userAccounts = await _accountRepository.GetAllUserAccountsAsync(userId);
            var userAccountsDto = new List<AccountDTO>(userAccounts.Select(account => new AccountDTO(account, null)));

            return userAccountsDto;
        }

        public async Task<decimal> GetUserBalance(ClaimsPrincipal userClaims)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            return await _accountRepository.GetUserBalanceAsync(userId);
        }

        public async Task<decimal> GetAccountBalance(ClaimsPrincipal userClaims, int accountId)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                throw new KeyNotFoundException($"Account ID:{accountId} was not found.");

            if (account.UserId != userId)
                throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to account ID:{accountId}.");

            return await _accountRepository.GetAccountBalanceAsync(accountId);
        }

        async Task<List<AccountDTO>> IAccountService.GetUserAccountsWithBalanceAsync(ClaimsPrincipal userClaims)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var userAccounts = await _accountRepository.GetUserAccountsWithBalanceAsync(userId);

            return userAccounts.Select(account => new AccountDTO(account.Item1, account.Item2)).ToList();
        }

        async Task<List<Tuple<int, decimal>>> IAccountService.GetBalanceForUserAccountsAsync(ClaimsPrincipal userClaims)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            return await _accountRepository.GetBalanceForUserAccountsAsync(userId);
        }
    }
}
