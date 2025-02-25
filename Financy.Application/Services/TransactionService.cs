using Application.IRepositories;
using Application.IServices;
using Domain;
using Domain.Interfaces;
using Financy.Application.DTOs.TransactionDTOs;
using System.Linq.Expressions;

namespace Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Transaction> AddTransactionAsync(CreateTransactionDTO transactionDto)
        {
            var account = await _accountRepository.GetByIdAsync(transactionDto.AccountId);
            if (account == null)
                throw new KeyNotFoundException($"Account with ID {transactionDto.AccountId} was not found.");

            var category = await _categoryRepository.GetByIdAsync(transactionDto.CategoryId);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {transactionDto.CategoryId} was not found.");

            var transaction = (Transaction)transactionDto;

            // Updating account balance
            account.AddTransactionToBalance(transaction);
            
            transaction.Account = account;
            transaction.Category = category;

            var addedTransaction = await _transactionRepository.CreateTransactionAsync(transaction);
            _accountRepository.UpdateAccount(account);
            await _unitOfWork.SaveChangesAsync();
            return addedTransaction;
        }


        public async Task<Transaction> UpdateTransactionAsync(EditTransactionDTO editedTransaction)
        {
            var transaction = await _transactionRepository.GetByIdAsync(editedTransaction.Id);

            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {editedTransaction.Id} was not found.");

            // When changing category
            if(transaction.CategoryId != editedTransaction.CategoryId)
            {
                var newCategory = await _categoryRepository.GetByIdAsync(editedTransaction.CategoryId);
                if (newCategory == null)
                    throw new KeyNotFoundException($"Category with ID {editedTransaction.CategoryId} was not found.");

                transaction.Category = newCategory;
            }

            // When changing amount
            if(transaction.Amount != editedTransaction.Amount)
            {
                var account = await _accountRepository.GetByIdAsync(transaction.AccountId);
                if (account == null)
                    throw new KeyNotFoundException($"Account with ID {transaction.AccountId} was not found.");

                account.UpdateBalance(transaction, editedTransaction);
                _accountRepository.UpdateAccount(account);
            }

            transaction.Amount = editedTransaction.Amount;
            transaction.Date = DateOnly.Parse(editedTransaction.Date);
            transaction.Description = editedTransaction.Description;

            var updatedTransaction = _transactionRepository.UpdateTransaction(transaction);
            await _unitOfWork.SaveChangesAsync();
            return updatedTransaction;

        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction != null)
            {
                _transactionRepository.DeleteTransaction(transaction);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _transactionRepository.GetAllTransactionsAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int transactionId)
        {
            return await _transactionRepository.GetByIdAsync(transactionId);
        }

        public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(Expression<Func<Transaction, bool>> predicate)
        {
            return await _transactionRepository.GetFilteredTransactionsAsync(predicate);
        }
    }
}
