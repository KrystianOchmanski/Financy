using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financy.Application.DTOs.TransactionDTOs
{
    public class EditTransferTransactionDTO
    {
        public Guid TransferGroupId { get; set; }
        [Range(1, int.MaxValue)]
        public int SourceAccountId { get; set; }
        [Range(1, int.MaxValue)]
        public int DestinationAccountId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string? Description { get; set; }
    }
}
