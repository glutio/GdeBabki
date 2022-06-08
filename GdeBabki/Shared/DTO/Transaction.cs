using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GdeBabki.Shared.DTO
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Tags { get; set; }
        public GBTransactionState State { get; set; }              
    }
}
