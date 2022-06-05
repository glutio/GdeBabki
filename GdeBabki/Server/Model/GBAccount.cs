using System;
using System.Collections.Generic;

namespace GdeBabki.Server.Model
{
    public class GBAccount: GBEntity
    {
        public string Name { get; set; }
        
        public Guid BankId { get; set; }
        public GBBank Bank { get; set; }

        public ICollection<GBTransaction> Transactions { get; set; }
    }
}
