using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdeBabki.Shared.DTO
{
    public class SharedTag
    {
        public string TagId { get; set; }
        public List<Guid> TransactionIds { get; set; }
    }
}
