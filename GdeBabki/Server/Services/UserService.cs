using GdeBabki.Server.Auth;
using GdeBabki.Server.Data;
using GdeBabki.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
                return Path.Combine(DBPath, DBName);
            }
        }

        public string DBPassword
        {
            get
            {
                var context = httpContextAccessor.HttpContext;
                if (context == null)
                {
                    return null;
                }

                var loginInfo = GBAuthentication.GetLoginInfoFromAuthHeader(context.Request.Headers.Authorization);
                if (loginInfo == null)
                {
                    return null;
                }

                return loginInfo.Password;
            }
        }
    }
}
