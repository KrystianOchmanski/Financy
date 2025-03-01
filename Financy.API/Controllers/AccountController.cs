using Financy.Application.DTOs.AccountDTOs;
using Financy.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financy.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAccount([FromBody] CreateAccountDTO createAccountDTO)
        {
            var account = await _accountService.AddUserAccountAsync(User, createAccountDTO);
            return CreatedAtAction(nameof(AddUserAccount), new { id = account.Id }, account);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAccounts()
        {
            var userAccounts = await _accountService.GetUserAccountsAsync(User);
            return Ok(userAccounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await _accountService.GetByIdAsync(User, id);
            return Ok(account);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var result = await _accountService.DeleteAccount(User, id);

            if(!result)
                return BadRequest($"Something went wrong when deleting an account ID:{id}.");

            return NoContent();
        }
    }
}
