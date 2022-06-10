using GdeBabki.Server.Model;
using Microsoft.EntityFrameworkCore;

namespace GdeBabki.Server.Data
{
    public class BabkiDbContext : DbContext
    {
        public BabkiDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GBTagGBTransaction>()
                .HasKey(e => new { e.TagId, e.TransactionId });
            
            //modelBuilder.Entity<GBTagGBTransaction>()
            //    .HasOne(e => e.Tag)
            //    .WithMany(e => e.Transactions)
            //    .HasForeignKey(e => e.TransactionId);

            //modelBuilder.Entity<GBTagGBTransaction>()
            //    .HasOne(e => e.Transaction)
            //    .WithMany(e => e.Tags)
            //    .HasForeignKey(e => e.TagId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<GBBank> Banks { get; set; }
        public DbSet<GBAccount> Accounts { get; set; }
        public DbSet<GBTransaction> Transactions { get; set; }
        public DbSet<GBTag> Tags { get; set; }
        public DbSet<GBTagGBTransaction> TagsTransactions { get; set; }
    }
}
