using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdeBabki.Shared.DTO
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Bank Bank { get; set; }
    }
}
