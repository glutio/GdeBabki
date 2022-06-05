using GdeBabki.Client.Services;
using GdeBabki.Server.Data;
using GdeBabki.Server.Model;
using GdeBabki.Shared;
using GdeBabki.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.IO;
using System.Linq;
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

        public async Task ImportAsync(Guid accountId, Stream stream, string filter)
        {
            var parser = new CsvParser();
            var lines = await parser.LoadAsync(stream);

            using (var db = await dbFactory.CreateDbContextAsync())
            {
                var account = await db.Accounts.FirstOrDefaultAsync(e => e.Id == accountId);
                
                var columnsFilter = filter.Split(",")
                    .Select(e => string.IsNullOrEmpty(e) ? GBColumns.Unknown : Enum.Parse<GBColumns>(e))
                    .ToArray();

                foreach (var line in lines)
                {
                    var transaction = ParseTransaction(line, columnsFilter);
                    if (transaction != null)
                    {
                        account.Transactions.Add(transaction);
                    }
                }
            }
        }

        private GBTransaction ParseTransaction(string[] line, GBColumns[] filter)
        {
            var transaction = new GBTransaction();
            
            for(var i = 0; i < filter.Length; i++)
            {
                switch(filter[i])
                {
                    case GBColumns.TransactionId:
                        transaction.TransactionId = line[i]; 
                        break;
                    case GBColumns.Amount:                         
                        if (decimal.TryParse(line[i], out decimal amount))
                        {
                            transaction.Amount = amount;
                            break;
                        };
                        return null;
                    case GBColumns.Description:
                        transaction.Description = line[i];
                        break;
                    case GBColumns.Date:
                        if (DateTime.TryParse(line[i], out DateTime dateTime))
                        {
                            transaction.Date = dateTime;
                            break;
                        }
                        return null;
                    case GBColumns.Unknown:
                        break;
                    default:
                        return null;
                }
            }

            return transaction;
        }
    }
}
