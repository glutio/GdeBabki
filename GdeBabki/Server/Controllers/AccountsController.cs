using GdeBabki.Server.Data;
using GdeBabki.Server.Services;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GdeBabki.Server.Controllers
{
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
        public async Task<IActionResult> DeleteAccountAsync(Guid accountId)
        {
            await accountsService.DeleteAccountAsync(accountId);
            return Ok();
        }

        [HttpGet("Banks")]
        public async Task<ActionResult<Bank>> GetBanksAsync()
        {
            var model = await accountsService.GetBanksAsync();
            return new JsonResult(model);
        }

        [HttpPost("Bank")]
        public async Task<ActionResult<Guid>> AddBankAsync(AddBank bank)
        {
            var model = await accountsService.AddBankAsync(bank);
            return new JsonResult(model);
        }

    }
}
