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

        public async Task<bool> UserHasDbAccess()
        {
            try
            {
                using var db = await dbFactory.CreateDbContextAsync();
                await db.Banks.CountAsync();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public async Task CreateDatabase()
        {
            var dbPath = userService.DBFilePath;
            if (File.Exists(dbPath))
            {
                throw new DuplicateNameException($"Database {dbPath} alredy exists");
            }

            using var db = await dbFactory.CreateDbContextAsync();
            await db.Database.MigrateAsync();
        }
    }
}
