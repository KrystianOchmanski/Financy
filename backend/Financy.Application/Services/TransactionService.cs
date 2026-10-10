using Domain;
using Financy.Application.DTOs.TransactionDTOs;
using Financy.Application.Exceptions;
using Financy.Application.IRepositories;
using Financy.Application.IServices;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Financy.Application.Services
{
	public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<TransactionDTO> AddTransactionAsync(ClaimsPrincipal userClaims, CreateTransactionDTO transactionDto)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var account = await _accountRepository.GetByIdAsync(transactionDto.AccountId);
            if (account == null)
                throw new KeyNotFoundException($"Account with ID {transactionDto.AccountId} was not found.");

            if (account.UserId != userId)
                throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to add transaction to account ID:{transactionDto.AccountId}");

            var category = await _categoryRepository.GetByIdAsync(transactionDto.CategoryId);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {transactionDto.CategoryId} was not found.");

            var transaction = (Transaction)transactionDto;
            
            transaction.Account = account;
            transaction.Category = category;

            var addedTransaction = await _transactionRepository.CreateTransactionAsync(transaction);
            await _unitOfWork.SaveChangesAsync();
            return addedTransaction;
        }


        public async Task<TransactionDTO> UpdateTransactionAsync(ClaimsPrincipal userClaims, EditTransactionDTO editedTransaction)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var transaction = await _transactionRepository.GetByIdAsync(editedTransaction.Id);

            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {editedTransaction.Id} was not found.");

            if (transaction.Account.UserId != userId)
                throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to update transaction with ID {editedTransaction.Id}");


            // When changing category
            if (transaction.CategoryId != editedTransaction.CategoryId)
            {
                var newCategory = await _categoryRepository.GetByIdAsync(editedTransaction.CategoryId);
                if (newCategory == null)
                    throw new KeyNotFoundException($"Category with ID {editedTransaction.CategoryId} was not found.");

                transaction.Category = newCategory;
            }

            transaction.Amount = editedTransaction.Amount;
            transaction.Date = DateOnly.Parse(editedTransaction.Date);
            transaction.Description = editedTransaction.Description;

            var updatedTransaction = _transactionRepository.UpdateTransaction(transaction);
            await _unitOfWork.SaveChangesAsync();
            return updatedTransaction;

        }

        public async Task DeleteTransactionAsync(ClaimsPrincipal userClaims, int id)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {id} was not found.");

            if(transaction.Account.UserId != userId)
                throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to delete transaction with ID {id}");

            _transactionRepository.DeleteTransaction(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<TransactionDTO>> GetAllUserTransactionsAsync(ClaimsPrincipal userClaims)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var userAccounts = await _accountRepository.GetAllUserAccountsAsync(userId);

            var userTransactions = new List<TransactionDTO>();
            foreach (var account in userAccounts)
            {
                userTransactions.AddRange(account.Transactions.Select(t => new TransactionDTO(t)));
            }

            return userTransactions.OrderByDescending(t => t.Date);
        }

        public async Task<IEnumerable<TransactionDTO>> GetLastUserTransactionsAsync(ClaimsPrincipal userClaims, int size)
        {
            var userId = _userManager.GetUserId(userClaims);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var userAccounts = await _accountRepository.GetAllUserAccountWithLastTransactionsAsync(userId, size);

            var userLastTransactions = new List<TransactionDTO>();
            foreach (var account in userAccounts)
            {
                userLastTransactions.AddRange(account.Transactions.Select(t => new TransactionDTO(t)));
            }

            return userLastTransactions.OrderByDescending(t => t.Date).Take(size);
        }

        public async Task<TransactionDTO?> GetTransactionByIdAsync(ClaimsPrincipal userClaims, int transactionId)
        {
			var userId = _userManager.GetUserId(userClaims);
			if (string.IsNullOrEmpty(userId))
				throw new UnauthorizedAccessException("Invalid token");

            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            if (transaction == null)
                return null;

            if (transaction.Account.UserId != userId)
                throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to transaction ID:{transactionId}");

            return transaction;
		}

        public async Task<IEnumerable<TransactionDTO>> GetFilteredTransactionsAsync(ClaimsPrincipal userClaims, TransactionFilterDTO filter)
        {
            var userTransactions = await GetAllUserTransactionsAsync(userClaims);

            var parsedStartDate = filter.GetParsedStartDate();
            var parsedEndDate = filter.GetParsedEndDate();

            var filteredTransactions = userTransactions
                .Where(t =>
                        (!filter.MinAmount.HasValue || t.Amount >= filter.MinAmount) &&
                        (!filter.MaxAmount.HasValue || t.Amount <= filter.MaxAmount) &&
                        (!parsedStartDate.HasValue || t.Date >= parsedStartDate) &&
                        (!parsedEndDate.HasValue || t.Date <= parsedEndDate) &&
                        (!filter.Type.HasValue || t.Type == filter.Type) &&
                        (!filter.AccountId.HasValue || t.AccountId == filter.AccountId) &&
                        (!filter.CategoryId.HasValue || t.CategoryId == filter.CategoryId)
				)
                //.OrderByDescending(t => t.Date)
                .ToList();

            return filteredTransactions;
        }
    }
}
