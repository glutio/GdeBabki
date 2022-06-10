using System.Collections.Generic;

namespace GdeBabki.Server.Model
{
    public class GBTag
    {
        public string Id { get; set; }
        public ICollection<GBTagGBTransaction> Transactions { get; set; }
    }
}
