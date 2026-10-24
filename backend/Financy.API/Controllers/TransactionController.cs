using Financy.Application.IServices;
using Financy.Application.DTOs.TransactionDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain;

namespace Financy.API.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserTransactions()
        {
            var transactions = await _transactionService.GetAllUserTransactionsAsync(User);
            return Ok(transactions);
        }

        [HttpGet("last")]
        public async Task<IActionResult> GetLastUserTransactions([FromQuery] int size)
        {
            var transactions = await _transactionService.GetLastUserTransactionsAsync(User, size);
            return Ok(transactions);
        }

        [HttpGet("incomes")]
        public async Task<IActionResult> GetIncomes(string startDate)
        {
            var filter = new TransactionFilterDTO { Type = TransactionType.Income, StartDate = startDate };
            var filteredTransactions = await _transactionService.GetFilteredTransactionsAsync(User, filter);
            decimal incomes = filteredTransactions.Aggregate((decimal)0, (sum, t) => sum + t.Amount);

            return Ok(incomes);
        }

        [HttpGet("expenses")]
        public async Task<IActionResult> GetExpenses(string startDate)
        {
            var filter = new TransactionFilterDTO { Type = TransactionType.Expense, StartDate = startDate };
            var filteredTransactions = await _transactionService.GetFilteredTransactionsAsync(User, filter);
            decimal expenses = filteredTransactions.Aggregate((decimal)0, (sum, t) => sum + t.Amount);

            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(User, id);

            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredTransactions([FromQuery] TransactionFilterDTO filter)
        {
            var filteredTransactions = await _transactionService.GetFilteredTransactionsAsync(User, filter);
            return Ok(filteredTransactions);
        }

        [HttpPost]
        public async Task<IActionResult> AddTransactionAsync([FromBody] CreateTransactionDTO transactionDTO)
        {
            var newTransaction = await _transactionService.AddTransactionAsync(User, transactionDTO);
            return CreatedAtAction(nameof(GetTransactionById), new { id = newTransaction.Id }, newTransaction);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> AddTransferTransactionAsync([FromBody] CreateTransferTransactionDTO transferTransactionDTO)
        {
            var newTransferTransactionGuid = await _transactionService.AddTransferTransactionAsync(User, transferTransactionDTO);
            return Ok(newTransferTransactionGuid);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransactionAsync(int id, [FromBody] EditTransactionDTO transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest($"Transaction ID mismatch. Request ID:{id} Body ID:{transaction.Id}");
            }

            var updatedTransaction = await _transactionService.UpdateTransactionAsync(User, transaction);

            if (updatedTransaction == null)
                return NotFound();

            return Ok(updatedTransaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionAsync(int id)
        {
            await _transactionService.DeleteTransactionAsync(User, id);
            return NoContent();
        }

        [HttpPut("transfer/{transferGroupId}")]
        public async Task<IActionResult> UpdateTransferTransactionAsync(Guid transferGroupId, [FromBody] EditTransferTransactionDTO transferTransactionDTO)
        {
            if (transferGroupId != transferTransactionDTO.TransferGroupId)
            {
                return BadRequest($"Transfer Group ID mismatch. Request ID:{transferGroupId} Body ID:{transferTransactionDTO.TransferGroupId}");
            }
            var updatedTransferTransactions = await _transactionService.UpdateTransferTransactionAsync(User, transferTransactionDTO);
            if (updatedTransferTransactions == null)
                return NotFound();
            return Ok(updatedTransferTransactions);
        }

        [HttpDelete("transfer/{transferGroupId}")]
        public async Task<IActionResult> DeleteTransferTransactionAsync(Guid transferGroupId)
        {
            await _transactionService.DeleteTransferTransactionAsync(User, transferGroupId);
            return NoContent();
        }
    }
}
