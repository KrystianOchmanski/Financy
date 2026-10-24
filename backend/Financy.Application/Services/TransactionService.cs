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

            if (transaction.TransferGroupId != null)
                throw new InvalidOperationException("Transfer transactions must be edited using the transfer endpoint.");

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

            //When changing account
            if (transaction.AccountId != editedTransaction.AccountId)
            {
                var newAccount = await _accountRepository.GetByIdAsync(editedTransaction.AccountId);
                if (newAccount == null)
                    throw new KeyNotFoundException($"Account with ID {editedTransaction.AccountId} was not found.");
                if (newAccount.UserId != userId)
                    throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to account with ID {editedTransaction.AccountId}");
                transaction.Account = newAccount;
            }

            transaction.Amount = editedTransaction.Amount;
            transaction.Date = DateOnly.Parse(editedTransaction.Date);
            transaction.Description = editedTransaction.Description;
            transaction.Type = editedTransaction.Type;

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
            if (transaction.TransferGroupId != null)
                throw new InvalidOperationException("Transfer transactions must be deleted using the transfer endpoint");

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

        async Task<Guid> ITransactionService.AddTransferTransactionAsync(ClaimsPrincipal user, CreateTransferTransactionDTO transferTransactionDTO)
        {
            var userId = _userManager.GetUserId(user);
            if(string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var sourceAccount = await _accountRepository.GetByIdAsync(transferTransactionDTO.SourceAccountId);
            if(sourceAccount == null)
                throw new KeyNotFoundException($"Source account with ID {transferTransactionDTO.SourceAccountId} was not found.");

            var destinationAccount = await _accountRepository.GetByIdAsync(transferTransactionDTO.DestinationAccountId);
            if(destinationAccount == null)
                throw new KeyNotFoundException($"Destination account with ID {transferTransactionDTO.DestinationAccountId} was not found.");

            if(sourceAccount.UserId != userId || destinationAccount.UserId != userId)
                throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to one or both of the accounts involved in the transfer.");

            if(sourceAccount.Id == destinationAccount.Id)
                throw new InvalidOperationException("Source and destination accounts cannot be the same.");

            var transferTransactionId = Guid.NewGuid();

            var sourceTransaction = new Transaction
            {
                Amount = transferTransactionDTO.Amount,
                Description = transferTransactionDTO.Description ?? "Transfer to " + destinationAccount.Name + " account",
                Date = transferTransactionDTO.Date ?? DateOnly.FromDateTime(DateTime.UtcNow),
                AccountId = transferTransactionDTO.SourceAccountId,
                Account = sourceAccount,
                CategoryId = null,
                Type = TransactionType.Expense,
                TransferDirection = TransferDirection.Outgoing,
                TransferGroupId = transferTransactionId,
            };

            var destinationTransaction = new Transaction
            {
                Amount = transferTransactionDTO.Amount,
                Description = transferTransactionDTO.Description ?? "Transfer from " + sourceAccount.Name + " account",
                Date = transferTransactionDTO.Date ?? DateOnly.FromDateTime(DateTime.UtcNow),
                AccountId = transferTransactionDTO.DestinationAccountId,
                Account = destinationAccount,
                CategoryId = null,
                Type = TransactionType.Income,
                TransferDirection = TransferDirection.Incoming,
                TransferGroupId = transferTransactionId,
            };

            await _transactionRepository.CreateTransactionAsync(sourceTransaction);
            await _transactionRepository.CreateTransactionAsync(destinationTransaction);
            await _unitOfWork.SaveChangesAsync();
            return transferTransactionId;
        }

        public async Task<IEnumerable<TransactionDTO>> UpdateTransferTransactionAsync(ClaimsPrincipal user, EditTransferTransactionDTO transferTransactionDTO)
        {
            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");
            if (transferTransactionDTO.SourceAccountId == transferTransactionDTO.DestinationAccountId)
                throw new InvalidOperationException("Source and destination accounts cannot be the same.");

            var sourceTransaction = await _transactionRepository.GetTransferTransactionByGroupIdAndDirection(transferTransactionDTO.TransferGroupId, TransferDirection.Outgoing);
            var destinationTransaction = await _transactionRepository.GetTransferTransactionByGroupIdAndDirection(transferTransactionDTO.TransferGroupId, TransferDirection.Incoming);

            if (sourceTransaction == null || destinationTransaction == null)
                throw new KeyNotFoundException($"Transfer transactions with group ID {transferTransactionDTO.TransferGroupId} were not found.");
            if (sourceTransaction.Account.UserId != userId || destinationTransaction.Account.UserId != userId)
                throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to one or both of the accounts involved in the transfer.");

            if (sourceTransaction.AccountId != transferTransactionDTO.SourceAccountId)
            {
                var newSourceAccount = await _accountRepository.GetByIdAsync(transferTransactionDTO.SourceAccountId);
                if (newSourceAccount == null)
                    throw new KeyNotFoundException($"Source account with ID {transferTransactionDTO.SourceAccountId} was not found.");
                if (newSourceAccount.UserId != userId)
                    throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to the source account with ID {transferTransactionDTO.SourceAccountId}");
                sourceTransaction.Account = newSourceAccount;
                sourceTransaction.AccountId = transferTransactionDTO.SourceAccountId;
                sourceTransaction.TransferDirection = TransferDirection.Outgoing;
            }
            if (destinationTransaction.AccountId != transferTransactionDTO.DestinationAccountId)
            {
                var newDestinationAccount = await _accountRepository.GetByIdAsync(transferTransactionDTO.DestinationAccountId);
                if (newDestinationAccount == null)
                    throw new KeyNotFoundException($"Destination account with ID {transferTransactionDTO.DestinationAccountId} was not found.");
                if (newDestinationAccount.UserId != userId)
                    throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to the destination account with ID {transferTransactionDTO.DestinationAccountId}");
                destinationTransaction.Account = newDestinationAccount;
                destinationTransaction.AccountId = transferTransactionDTO.DestinationAccountId;
                destinationTransaction.TransferDirection = TransferDirection.Incoming;
            }

            sourceTransaction.Amount = transferTransactionDTO.Amount;
            sourceTransaction.Date = transferTransactionDTO.Date;
            sourceTransaction.Description = transferTransactionDTO.Description ?? sourceTransaction.Description;
            destinationTransaction.Amount = transferTransactionDTO.Amount;
            destinationTransaction.Date = transferTransactionDTO.Date;
            destinationTransaction.Description = transferTransactionDTO.Description ?? destinationTransaction.Description;

            _transactionRepository.UpdateTransaction(sourceTransaction);
            _transactionRepository.UpdateTransaction(destinationTransaction);
            await _unitOfWork.SaveChangesAsync();
            return new List<TransactionDTO>() { new TransactionDTO(sourceTransaction), new TransactionDTO(destinationTransaction) };
        }

        async Task ITransactionService.DeleteTransferTransactionAsync(ClaimsPrincipal user, Guid transferGroupId)
        {
            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            var sourceTransaction = await _transactionRepository.GetTransferTransactionByGroupIdAndDirection(transferGroupId, TransferDirection.Outgoing);
            var destinationTransaction = await _transactionRepository.GetTransferTransactionByGroupIdAndDirection(transferGroupId, TransferDirection.Incoming);

            if (sourceTransaction == null || destinationTransaction == null)
                throw new KeyNotFoundException($"Transfer transactions with group ID {transferGroupId} were not found.");
            if (sourceTransaction.Account.UserId != userId || destinationTransaction.Account.UserId != userId)
                throw new ForbiddenAccessException($"User (ID:{userId}) does not have access to one or both of the accounts involved in the transfer.");

            _transactionRepository.DeleteTransaction(sourceTransaction);
            _transactionRepository.DeleteTransaction(destinationTransaction);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
