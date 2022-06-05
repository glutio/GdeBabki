using GdeBabki.Server.Data;
using GdeBabki.Server.Model;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GdeBabki.Server.Services
{
    public class AccountsService
    {
        private readonly IDbContextFactory<BabkiDbContext> dbFactory;

        public AccountsService(IDbContextFactory<BabkiDbContext> dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public async Task<Account[]> GetAccountsAsync()
        {
            using var db = await dbFactory.CreateDbContextAsync();
            var accounts = await db.Accounts
                .Select(e => new Account()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Bank = new Bank()
                    {
                        Id = e.Bank.Id,
                        Name = e.Bank.Name
                    }
                })
                .ToArrayAsync();
            return accounts;
        }

        public async Task<Guid> UpsertAccountAsync(UpsertAccount account)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            GBAccount gbAccount;
            if (account.AccountId != Guid.Empty)
            {
                gbAccount = await db.Accounts.FirstOrDefaultAsync(e => e.Id == account.AccountId);
            }
            else
            {
                gbAccount = new GBAccount();
                db.Accounts.Add(gbAccount);
            }

            gbAccount.Name = account.Name;
            gbAccount.BankId = account.BankId;

            db.SaveChanges();

            return gbAccount.Id;
        }

        public async Task DeleteAccountAsync([FromQuery]Guid accountId)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            var gbAccount = new GBAccount()
            {
                Id = accountId
            };
            db.Accounts.Remove(gbAccount);
            
            db.SaveChanges();
        }

        public async Task<Bank[]> GetBanksAsync()
        {
            using var db = await dbFactory.CreateDbContextAsync();
            var banks = await db.Banks
                .Select(e => new Bank()
                {
                    Id = e.Id,
                    Name = e.Name,
                })
                .ToArrayAsync();

            return banks;
        }

        public async Task<Guid> AddBankAsync(AddBank bank)
        {
            using var db = await dbFactory.CreateDbContextAsync();
            var gbBank = new GBBank()
            {
                Name = bank.Name,
            };

            db.Banks.Add(gbBank);
            db.SaveChanges();

            return gbBank.Id;
        }

    }
}
