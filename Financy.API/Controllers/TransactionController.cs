using Application.IServices;
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
            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> AddTransactionAsync([FromBody] CreateTransactionDTO transactionDTO)
        {
            var newTransaction = await _transactionService.AddTransactionAsync(User, transactionDTO);
            return CreatedAtAction(nameof(_transactionService.AddTransactionAsync), newTransaction);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransactionAsync([FromBody] EditTransactionDTO transaction)
        {
            var updatedTransaction = await _transactionService.UpdateTransactionAsync(User, transaction);
            return Ok(updatedTransaction);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTransactionAsync(int id)
        {
            await _transactionService.DeleteTransactionAsync(User, id);
            return NoContent();
        }
    }
}
