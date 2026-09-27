using Domain;
using Financy.Application.DTOs.TransactionDTOs;

namespace Financy.Application.DTOs.AccountDTOs
{
    public class AccountDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public List<TransactionDTO> Transactions { get; set; } = new List<TransactionDTO>();

        public AccountDTO(Account account) 
        { 
            Id = account.Id;
            Name = account.Name;
            Balance = account.Balance;
            Transactions = account.Transactions.Select(t => new TransactionDTO(t)).ToList();
        }

        public static implicit operator AccountDTO(Account account)
        {
            return new AccountDTO(account);
        }
    }
}
