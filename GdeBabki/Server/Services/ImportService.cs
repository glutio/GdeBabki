using GdeBabki.Client.Services;
using GdeBabki.Server.Data;
using GdeBabki.Server.Model;
using GdeBabki.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdeBabki.Server.Services
{
    public class ImportService
    {
        private IDbContextFactory<BabkiDbContext> dbFactory;

        public ImportService(IDbContextFactory<BabkiDbContext> dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public async Task ImportAsync(Guid accountId, Stream stream, GBColumnName?[] filter)
        {
            var parser = new CsvParser();
            var lines = await parser.LoadAsync(stream);

            using var db = await dbFactory.CreateDbContextAsync();
            using var dbTransaction = await db.Database.BeginTransactionAsync();
            foreach (var line in lines)
            {
                var transaction = ParseTransaction(line, filter);
                if (transaction != null)
                {
                    transaction.AccountId = accountId;
                    transaction.Id = transaction.GetMD5();

                    var isExisting = await db.Transactions.AnyAsync(e => e.Id == transaction.Id);
                    if (!isExisting)
                    {
                        db.Transactions.Add(transaction);
                    }
                }
            }
            await db.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        }

        private GBTransaction ParseTransaction(string[] line, GBColumnName?[] filter)
        {
            var transaction = new GBTransaction();
            
            for (var i = 0; i < filter.Length; i++)
            {
                if (filter[i] != null && line.Length <= i)
                {
                    return null;
                }

                switch (filter[i])
                {
                    case GBColumnName.TransactionId:
                        transaction.TransactionId = line[i];
                        break;
                    case GBColumnName.Amount:
                        var sb = new StringBuilder();
                        foreach(var c in line[i])
                        {
                            if (c=='-' || c == '.' || (c >= '0' && c <= '9'))
                            {
                                sb.Append(c);
                            }
                        }
                        if (decimal.TryParse(sb.ToString(), out decimal amount))
                        {
                            transaction.Amount = amount;
                            break;
                        };
                        return null;
                    case GBColumnName.Description:
                        transaction.Description = line[i];
                        break;
                    case GBColumnName.Date:
                        if (DateTime.TryParse(line[i], out DateTime dateTime))
                        {
                            transaction.Date = dateTime;
                            break;
                        }
                        return null;
                    case null:
                        break;
                    default:
                        return null;
                }
            }

            return transaction;
        }
    }
}
