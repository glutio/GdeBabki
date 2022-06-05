using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdeBabki.Shared.DTO
{
    public class UpsertAccount
    {        
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public Guid BankId { get; set; }
    }
}
