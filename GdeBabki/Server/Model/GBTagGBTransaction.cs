using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GdeBabki.Server.Model
{
    public class GBTagGBTransaction
    {
        public string TagId { get; set; }
        public GBTag Tag { get; set; }

        public Guid TransactionId { get; set; }
        public GBTransaction Transaction { get; set; }
    }
}
