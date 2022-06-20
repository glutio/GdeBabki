using GdeBabki.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace GdeBabki.Server.Services
{
    public class DatabaseService
    {
        private readonly IDbContextFactory<BabkiDbContext> dbFactory;
        private readonly UserService userService;

        public DatabaseService(IDbContextFactory<BabkiDbContext> dbFactory, UserService userService)
        {
            this.dbFactory = dbFactory;
            this.userService = userService;
        }

        public async Task<bool> UserHasDbAccessAsync()
        {
            try
            {
                using var db = await dbFactory.CreateDbContextAsync();
                await db.Banks.CountAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task CreateDatabaseAsync()
        {
            var dbFilePath = userService.DBFilePath;
            if (File.Exists(dbFilePath))
            {
                throw new DuplicateNameException($"Database {dbFilePath} alredy exists");
            }
            
            var options = new DbContextOptionsBuilder<BabkiDbContext>();
            userService.SetubDbContextOptions(options, true);
            using var db = new BabkiDbContext(options.Options);
            await db.Database.MigrateAsync();
        }
    }
}
