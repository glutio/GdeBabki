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

        public async Task ImportAsync(Guid accountId, Stream stream, GBColumnName?[] filter)
        {
            var parser = new CsvParser();
            var lines = await parser.LoadAsync(stream);

            using var db = await dbFactory.CreateDbContextAsync();
            var account = await db.Accounts.FirstOrDefaultAsync(e => e.Id == accountId);
                
            foreach (var line in lines)
            {
                var transaction = ParseTransaction(line, filter);
                if (transaction != null)
                {
                    account.Transactions.Add(transaction);
                }
            }
        }

        private GBTransaction ParseTransaction(string[] line, GBColumnName?[] filter)
        {
            var transaction = new GBTransaction();
            
            for(var i = 0; i < filter.Length; i++)
            {
                switch(filter[i])
                {
                    case GBColumnName.TransactionId:
                        transaction.TransactionId = line[i]; 
                        break;
                    case GBColumnName.Amount:                         
                        if (decimal.TryParse(line[i], out decimal amount))
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
