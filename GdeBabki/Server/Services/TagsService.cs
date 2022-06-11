using GdeBabki.Server.Data;
using GdeBabki.Server.Model;
using GdeBabki.Shared.DTO;
using Microsoft.EntityFrameworkCore;
using System;
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
    }
}
