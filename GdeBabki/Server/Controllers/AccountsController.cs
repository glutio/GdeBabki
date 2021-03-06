using GdeBabki.Server.Data;
using GdeBabki.Server.Services;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GdeBabki.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AccountsService accountsService;

        public AccountsController(AccountsService accountsService)
        {
            this.accountsService = accountsService;
        }

        [HttpGet]
        public async Task<ActionResult<Account[]>> GetAccountsAsync()
        {
            var model = await accountsService.GetAccountsAsync();
            return new JsonResult(model);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> UpsertAccountAsync(UpsertAccount account)
        {
            var model = await accountsService.UpsertAccountAsync(account);
            return new JsonResult(model);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccountAsync(Guid id)
        {
            await accountsService.DeleteAccountAsync(id);
            return Ok();
        }

        [HttpGet("Banks")]
        public async Task<ActionResult<Bank>> GetBanksAsync()
        {
            var model = await accountsService.GetBanksAsync();
            return new JsonResult(model);
        }

        [HttpPost("Banks")]
        public async Task<ActionResult<Guid>> UpsertBankAsync(Bank bank)
        {
            var model = await accountsService.UpsertBankAsync(bank);
            return new JsonResult(model);
        }

        [HttpDelete("Banks")]
        public async Task<IActionResult> DeleteBankAsync([FromQuery] Guid bankId)
        {
            await accountsService.DeleteBankAsync(bankId);
            return Ok();
        }

        [HttpGet("Transactions")]
        public async Task<ActionResult<Transaction[]>> GetTransactionsAsync([FromQuery] Guid[] accountIds)
        {
            var model = await accountsService.GetTransactionsAsync(accountIds);
            return new JsonResult(model);
        }

        [HttpPost("Transaction")]
        public async Task<ActionResult<Guid>> UpsertTransactionAsync(Transaction transaction)
        {
            var model = await accountsService.UpsertTransactionAsync(transaction);
            return model;
        }

        [HttpDelete("Transactions")]
        public async Task<IActionResult> DeleteTransactionsAsync([FromBody]Guid[] transactionIds)
        {
            await accountsService.DeleteTransactionsAsync(transactionIds);
            return Ok();
        }
    }
}
