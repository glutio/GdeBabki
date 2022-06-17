using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdeBabki.Shared.DTO
{
    public class LoginInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public static bool TryParse(string loginString, out LoginInfo loginInfo)
        {
            loginInfo = null;
            var items = loginString.Split(',');
            if (items == null || items.Length != 2)
            {
                return false;
            }

            loginInfo = new LoginInfo()
            {
                UserName = items[0],
                Password = items[1]
            };

            return true;
        }

        public override string ToString()
        {
            return $"{UserName},{Password}";
        }
    }
}
