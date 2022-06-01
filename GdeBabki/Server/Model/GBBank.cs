using System.Collections.Generic;

namespace GdeBabki.Server.Model
{
    public class GBBank: GBEntity
    {
        public string Name { get; set; }
        public IReadOnlyCollection<GBAccount> Accounts { get; set; }
    }
}