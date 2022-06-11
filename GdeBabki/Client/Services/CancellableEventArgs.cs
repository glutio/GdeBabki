using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace GdeBabki.Client.Services
{
    public class CancelEventArgs<TValue>: CancelEventArgs
    {
        public CancelEventArgs()
        {
            Cancel = false;
        }

        public CancelEventArgs(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; set; }
    }
}
