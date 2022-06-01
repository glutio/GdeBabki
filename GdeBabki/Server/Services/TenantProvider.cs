using System.IO;
using System.Linq;
using System.Security.Claims;

namespace GdeBabki.Server.Services
{
    public class TenantProvider : ITenantProvider
    {
        const string DEFAULT_DB = "GdeBabkiUser";

        readonly ClaimsPrincipal _claimsPrincipal;
        public TenantProvider(ClaimsPrincipal claimsPrincipal)
        {
            _claimsPrincipal = claimsPrincipal;
        }
        public string DBName
        {
            get
            {
                var email = _claimsPrincipal?.Claims?.FirstOrDefault(e => e.Type == ClaimTypes.Email)?.Value;
                var dbName = string.IsNullOrEmpty(email)
                        ? DEFAULT_DB
                        : string.Concat(email.Split(Path.GetInvalidFileNameChars()));

                return dbName;
            }
        }
    }
}
