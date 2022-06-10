using System.Collections.Generic;

namespace GdeBabki.Server.Model
{
    public class GBTag
    {
        public string Id { get; set; }
        public List<GBTransaction> Transactions { get; set; }
    }
}
