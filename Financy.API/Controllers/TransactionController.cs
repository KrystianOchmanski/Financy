using Application.IServices;
using Domain;
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
            return Ok(newTransaction);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransactionAsync([FromBody] EditTransactionDTO transaction)
        {
            var updatedTransaction = await _transactionService.UpdateTransactionAsync(transaction);
            return Ok(updatedTransaction);
        }
    }
}
