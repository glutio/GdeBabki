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
    public class TagsService
    {
        private readonly IDbContextFactory<BabkiDbContext> dbFactory;

        public TagsService(IDbContextFactory<BabkiDbContext> dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public async Task InsertTagAsync(TransactionTag insertTag)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            var isExisting = await db.Tags.AnyAsync(e => e.Id == insertTag.TagId);
            if (!isExisting)
            {
                db.Tags.Add(new GBTag() { Id = insertTag.TagId });
            }
            //isExisting = await db.TagsTransactions.AnyAsync(e => e.TagId == insertTag.TagId && e.TransactionId == insertTag.TransactionId);
            //if (!isExisting)
            //{
            db.TagsTransactions.Add(new GBTagGBTransaction() { TagId = insertTag.TagId, TransactionId = insertTag.TransactionId });
            //}

            await db.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(string tagId, Guid transactionId)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            db.TagsTransactions.Remove(new GBTagGBTransaction()
            {
                TagId = tagId,
                TransactionId = transactionId
            });

            await db.SaveChangesAsync();
        }

        public async Task InsertSharedTagAsync(SharedTag sharedTag)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            using var dbTransaction = await db.Database.BeginTransactionAsync();

            var isExisting = await db.Tags.AnyAsync(e => e.Id == sharedTag.TagId);
            if (!isExisting)
            {
                db.Tags.Add(new GBTag() { Id = sharedTag.TagId });
            }

            foreach(var transactionId in sharedTag.TransactionIds)
            {
                db.TagsTransactions.Add(new GBTagGBTransaction() { TagId = sharedTag.TagId, TransactionId = transactionId });
            }

            await db.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        }

        public async Task DeleteSharedTagAsync(SharedTag sharedTag)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            using var dbTransaction = await db.Database.BeginTransactionAsync();
            foreach(var transactionId in sharedTag.TransactionIds)
            {
                db.TagsTransactions.Remove(new GBTagGBTransaction()
                {
                    TagId = sharedTag.TagId,
                    TransactionId = transactionId
                });
            }

            await db.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        }

        public async Task<string[]> GetSuggestedTagsAsync(Guid transactionId)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            var topTags = await db.TagsTransactions
                .GroupBy(e => e.TagId)
                .Select(g => new
                {
                    TagId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(e => e.Count)
                .Select(e => e.TagId)
                .Take(7)
                .ToArrayAsync();

            return topTags;
        }


    }
}
