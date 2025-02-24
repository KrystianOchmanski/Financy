using Application.DTOs;
using Application.IRepositories;
using Application.IServices;
using Domain;
using Domain.Interfaces;
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
        public async Task<Transaction> AddTransactionAsync(AddTransactionDTO transactionDto)
        {
            var account = await _accountRepository.GetByIdAsync(transactionDto.AccountId);
            if (account == null)
                throw new Exception("Account not found");

            var category = await _categoryRepository.GetByIdAsync(transactionDto.CategoryId);
            if (category == null)
                throw new Exception("Category not found");

            // Updating account balance
            switch (transactionDto.Type)
            {
                case TransactionType.Income:
                    account.Balance += transactionDto.Amount;
                    break;

                case TransactionType.Expense:
                    if (account.Balance < transactionDto.Amount)
                    {
                        throw new Exception("Insufficient balance");
                    }
                    account.Balance -= transactionDto.Amount;
                    break;
            }

            var transaction = (Transaction)transactionDto;
            transaction.Account = account;
            transaction.Category = category;

            var addedTransaction = await _transactionRepository.CreateTransactionAsync(transaction);
            _accountRepository.UpdateAccount(account);
            await _unitOfWork.SaveChangesAsync();
            return addedTransaction;
        }


        public async void UpdateTransaction(Transaction transaction)
        {
            _transactionRepository.UpdateTransactionAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

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
