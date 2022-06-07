using GdeBabki.Server.Model;
using Microsoft.EntityFrameworkCore;

namespace GdeBabki.Server.Data
{
    public class BabkiDbContext : DbContext
    {
        public BabkiDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<GBBank> Banks { get; set; }
        public DbSet<GBAccount> Accounts { get; set; }
        public DbSet<GBTransaction> Transactions { get;set; }
    }
}
