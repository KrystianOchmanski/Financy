using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Account name is required")]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        [JsonIgnore]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public string UserId { get; set; } = null!;

        [JsonIgnore]
        public User User { get; set; } = null!;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        public void AddTransactionToBalance(Transaction transaction)
        {
            switch (transaction.Type)
            {
                case TransactionType.Income:
                    Balance += transaction.Amount;
                    break;

                case TransactionType.Expense:
                    if (Balance < transaction.Amount)
                    {
                        throw new InvalidOperationException("Insufficient balance");
                    }
                    Balance -= transaction.Amount;
                    break;
            }
        }

        public void RemoveTransactionFromBalance(Transaction transaction)
        {
            switch (transaction.Type)
            {
                case TransactionType.Income:
                    if (Balance - transaction.Amount < 0)
                    {
                        throw new InvalidOperationException("Cannot revert transaction as it would result in negative balance");
                    }
                    Balance -= transaction.Amount;
                    break;
                case TransactionType.Expense:
                    Balance += transaction.Amount;
                    break;
            }
        }

        public void UpdateBalance(Transaction oldTransaction, Transaction newTransaction)
        {
            decimal amountDifference = oldTransaction.Amount - newTransaction.Amount;
            switch (oldTransaction.Type)
            {
                case TransactionType.Income:
                    if (Balance - amountDifference < 0)
                    {
                        throw new InvalidOperationException("Cannot revert transaction as it would result in negative balance");
                    }
                    Balance -= amountDifference;
                    break;
                case TransactionType.Expense:
                    Balance += amountDifference;
                    break;
            }
        }
    }
}
