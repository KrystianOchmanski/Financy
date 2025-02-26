using Application.IServices;
using Domain;
using Financy.Application.DTOs.TransactionDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(User, id);
            
            if(transaction == null)
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
			if (!string.IsNullOrEmpty(transactionDTO.Date) && !DateOnly.TryParse(transactionDTO.Date, out _))
			{
				return BadRequest("Invalid Date format. Use YYYY-MM-DD.");
			}

			var newTransaction = await _transactionService.AddTransactionAsync(User, transactionDTO);
			return CreatedAtAction(nameof(GetTransactionById), new { id = newTransaction.Id }, newTransaction);
		}

		[HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransactionAsync(int id, [FromBody] EditTransactionDTO transaction)
        {
            if(id != transaction.Id)
            {
                return BadRequest("Transaction ID mismatch");
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
    }
}
