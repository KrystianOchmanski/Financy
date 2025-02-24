using Application.DTOs;
using Application.IServices;
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
        public async Task<IActionResult> AddTransactionAsync([FromBody] AddTransactionDTO transactionDTO)
        {
            //if (transactionDTO == null)
            //{
            //    return BadRequest();
            //}
            var newTransaction = await _transactionService.AddTransactionAsync(transactionDTO);
            return Ok(newTransaction);
        }
    }
}
