using System.Security.Principal;

namespace GdeBabki.Server.Auth
{
    public class GBIdentity : IIdentity
    {
        public string AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }
    }
}
