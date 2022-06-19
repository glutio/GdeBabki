using GdeBabki.Server.Auth;
using GdeBabki.Server.Data;
using GdeBabki.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace GdeBabki.Server.Services
{
    public class UserService
    {
        const string DEFAULT_DB = "GdeBabkiUser";

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;

        public UserService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.webHostEnvironment = webHostEnvironment;
        }

        public string DBName
        {
            get
            {
                var loginInfo = GBAuthentication.GetLoginInfoFromAuthHeader(httpContextAccessor.HttpContext?.Request?.Headers?.Authorization);
                if (!webHostEnvironment.IsDevelopment() && (string.IsNullOrEmpty(loginInfo?.UserName)))
                {
                    throw new ArgumentException("User name cannot be empty");
                }

                var dbName = string.IsNullOrEmpty(loginInfo?.UserName)
                        ? DEFAULT_DB
                        : string.Concat(loginInfo.UserName.Split(Path.GetInvalidFileNameChars()));

                return dbName;
            }
        }

        public string DBPath
        {
            get
            {
                var path = webHostEnvironment.IsDevelopment()
                    ? ""
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return path;
            }
        }

        public string DBFilePath
        {
            get
            {
                return $"{Path.Combine(DBPath, DBName)}.sqlite";
            }
        }

        public string DBPassword
        {
            get
            {
                var loginInfo = GBAuthentication.GetLoginInfoFromAuthHeader(httpContextAccessor.HttpContext?.Request?.Headers?.Authorization);
                if (!webHostEnvironment.IsDevelopment() && (string.IsNullOrEmpty(loginInfo?.Password)))
                {
                    throw new ArgumentException("Password cannot be empty");
                }

                return loginInfo?.Password;
            }
        }

        public void SetubDbContextOptions(DbContextOptionsBuilder options, bool canCreate)
        {
            var baseConnectionString = $"Data Source={DBFilePath}";
            var connectionString = new SqliteConnectionStringBuilder(baseConnectionString)
            {
                Mode = canCreate ? SqliteOpenMode.ReadWriteCreate : SqliteOpenMode.ReadWrite,
                Password = DBPassword
            }.ToString();

            options
                .UseSqlite(connectionString)
                //.LogTo(Console.WriteLine, LogLevel.Information)
                ;
        }
    }
}
