using GdeBabki.Shared.DTO;
using System.Linq;
using System.Net.Http.Headers;

namespace GdeBabki.Shared
{
    public class GBAuthentication
    {
        public const string AuthenticationScheme = "GdeBabkiAuth";

        public static AuthenticationHeaderValue GetAuthHeaderFromLoginInfo(LoginInfo loginInfo)
        {
            if (loginInfo == null)
            {
                return null;
            }

            var header = $"{loginInfo.UserName},{loginInfo.Password}";
            return new(AuthenticationScheme, header);
        }

        public static LoginInfo GetLoginInfoFromAuthHeader(string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader))
            {
                return null;
            }

            var items = authHeader.Split(' ');
            if (items.Length != 2 || items[0] != AuthenticationScheme)
            {
                return null;
            }

            if (LoginInfo.TryParse(items[1], out LoginInfo loginInfo))
            {
                return loginInfo;
            }

            return null;
        }
    }
}
