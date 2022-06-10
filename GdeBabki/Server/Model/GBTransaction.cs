using GdeBabki.Shared;
using System;
using System.Collections.Generic;

namespace GdeBabki.Server.Model
{
    public class GBTransaction: GBEntity
    {
        public Guid AccountId { get; set; }
        public GBAccount Account { get; set; }
        public string TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public ICollection<GBTagGBTransaction> Tags { get; set; }
        public GBTransactionState State { get; set; }
    }
}