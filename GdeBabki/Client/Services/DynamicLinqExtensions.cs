using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace GdeBabki.Client.Services
{
    [DynamicLinqType]
    public static class DynamicLinqExtensions
    {
        public static bool Contains(this object list, object s)
        {       
            return true;
        }

    }
}
