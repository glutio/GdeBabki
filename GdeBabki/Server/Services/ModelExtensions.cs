using GdeBabki.Server.Model;
using System;
using System.Security.Cryptography;

namespace GdeBabki.Server.Services
{
    public static class ModelExtensions
    {
        public static Guid GetMD5(this GBTransaction t)
        {
            using var md5 = MD5.Create();
            var all = $"{t.AccountId}{t.TransactionId}{t.Date}{t.Description}{t.Amount}";
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(all);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }
    }
}
