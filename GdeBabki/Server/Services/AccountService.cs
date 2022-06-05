using GdeBabki.Server.Data;
using GdeBabki.Server.Model;
using GdeBabki.Shared.DTO;
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

        public async Task<Guid> AddAccountAsync(AddAccount account)
        {
            using var db = await dbFactory.CreateDbContextAsync();
            var gbAccount = new GBAccount()
            {
                Name = account.Name,
                BankId = account.BankId
            };

            db.Accounts.Add(gbAccount);
            db.SaveChanges();

            return gbAccount.Id;
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
