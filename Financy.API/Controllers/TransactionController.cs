using Application.IServices;
using Financy.Application.DTOs.TransactionDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> AddTransactionAsync([FromBody] CreateTransactionDTO transactionDTO)
        {
            var newTransaction = await _transactionService.AddTransactionAsync(transactionDTO);
            return CreatedAtAction("AddTransactionAsync", newTransaction);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransactionAsync([FromBody] EditTransactionDTO transaction)
        {
            var updatedTransaction = await _transactionService.UpdateTransactionAsync(transaction);
            return Ok(updatedTransaction);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTransactionAsync(int id)
        {
            await _transactionService.DeleteTransactionAsync(id);
            return NoContent();
        }
    }
}
