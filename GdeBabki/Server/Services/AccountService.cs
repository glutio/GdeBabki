using GdeBabki.Server.Data;
using GdeBabki.Server.Model;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

            GBAccount gbAccount = new GBAccount()
            {
                Id = account.AccountId
            };

            if (account.AccountId != Guid.Empty)
            {
                db.Accounts.Attach(gbAccount);
            }
            else
            {
                db.Accounts.Add(gbAccount);
            }

            gbAccount.Name = account.Name;
            gbAccount.BankId = account.BankId;

            await db.SaveChangesAsync();

            return gbAccount.Id;
        }

        public async Task DeleteAccountAsync([FromQuery] Guid id)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            var gbAccount = new GBAccount()
            {
                Id = id
            };
            db.Accounts.Remove(gbAccount);

            await db.SaveChangesAsync();
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

        public async Task<Guid> UpsertTransactionAsync(Transaction transaction)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            GBTransaction gbTransaction;
            if (transaction.Id != Guid.Empty)
            {
                gbTransaction = await db.Transactions.FirstOrDefaultAsync(e => e.Id == transaction.Id);
            }
            else
            {
                gbTransaction = new();
                db.Transactions.Add(gbTransaction);
            }

            gbTransaction.Amount = transaction.Amount;
            gbTransaction.State = transaction.State;
            gbTransaction.TransactionId = transaction.TransactionId;
            gbTransaction.Date = transaction.Date;
            gbTransaction.Description = transaction.Description;

            var oldTags = db.TagsTransactions.Where(e => e.TransactionId == transaction.Id).ExceptBy(transaction.Tags, e => e.TagId);
            db.TagsTransactions.RemoveRange(oldTags);

            foreach(var tag in transaction.Tags)
            {
                var isExisting = await db.Tags.AnyAsync(e => e.Id == tag);
                if (!isExisting)
                {
                    db.Tags.Add(new GBTag() { Id = tag });
                }

                isExisting = await db.TagsTransactions.AnyAsync(e => e.TagId == tag);
                if (!isExisting)
                {
                    db.TagsTransactions.Add(new GBTagGBTransaction() { TagId = tag, TransactionId = transaction.Id });
                }
            }

            await db.SaveChangesAsync();

            return gbTransaction.Id;
        }

        public async Task DeleteTransactionsAsync(Guid[] transactionIds)
        {
            using var db = await dbFactory.CreateDbContextAsync();
            
            using var dbTransaction = await db.Database.BeginTransactionAsync();
            foreach (var transactionId in transactionIds)
            {
                var gbTransaction = new GBTransaction()
                {
                    Id = transactionId,
                };
                db.Transactions.Attach(gbTransaction);
                
                gbTransaction.IsDeleted = true;
            }

            await db.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        }

        public async Task<Transaction[]> GetTransactionsAsync(Guid[] accountIds)
        {
            using var db = await dbFactory.CreateDbContextAsync();
            var transactions = await db.Transactions.Include(e => e.Tags)
                .Where(e => e.IsDeleted == false)                
                .Where(e => accountIds == null || accountIds.Length == 0 || accountIds.Any(id => id == e.AccountId))
                .Select(e => new Transaction()
                {
                    Id = e.Id,
                    Description = e.Description,
                    Amount = e.Amount,
                    Date = e.Date,
                    State = e.State,
                    Tags = e.Tags.Select(e => e.TagId).ToList(),
                    TransactionId = e.TransactionId
                })
                .OrderByDescending(e => e.Date)
                .ToArrayAsync();

            return transactions;
        }

        public async Task<Guid> UpsertBankAsync(Bank bank)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            GBBank gbBank;
            if (bank.Id != Guid.Empty)
            {
                gbBank = await db.Banks.FirstOrDefaultAsync(e => e.Id == bank.Id);
            }
            else
            {
                gbBank = new GBBank();
                db.Banks.Add(gbBank);
            }

            gbBank.Name = bank.Name;

            await db.SaveChangesAsync();

            return gbBank.Id;
        }

        public async Task DeleteBankAsync(Guid bankId)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            var gbBank = new GBBank()
            {
                Id = bankId
            };
            db.Banks.Remove(gbBank);

            await db.SaveChangesAsync();
        }

    }
}
