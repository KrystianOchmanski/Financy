using Domain;

namespace Financy.Application.DTOs.TransactionDTOs
{
    public class TransactionDTO
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateOnly Date {  get; set; }

        public string Description { get; set; } = string.Empty;

        public TransactionType Type { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public TransactionDTO(Transaction transaction)
        {
            Id = transaction.Id;
            Amount = transaction.Amount;
            Date = transaction.Date;
            Description = transaction.Description;
            Type = transaction.Type;
            AccountId = transaction.AccountId;
            AccountName = transaction.Account.Name;
            CategoryId = transaction.CategoryId;
            CategoryName = transaction.Category.Name;
        }

        public static implicit operator TransactionDTO(Transaction transaction)
        {
            return new TransactionDTO(transaction);
        }
    }
}
