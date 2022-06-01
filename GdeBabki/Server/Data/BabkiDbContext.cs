using GdeBabki.Server.Model;
using Microsoft.EntityFrameworkCore;

namespace GdeBabki.Server.Data
{
    public class BabkiDbContext : DbContext
    {
        public BabkiDbContext(DbContextOptions options) : base(options)
        {
        }

        DbSet<GBBank> Banks { get; set; }
        DbSet<GBAccount> Accounts { get; set; }
    }
}
