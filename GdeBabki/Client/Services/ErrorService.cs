using GdeBabki.Client.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GdeBabki.Client.Services
{
    public class ErrorService
    {
        public List<ErrorMessage> errors = new();
        public IEnumerable<ErrorMessage> Errors => errors;

        public event EventHandler ErrorUpdated;

        public ErrorService()
        {
        }

        public void AddError(string message, int msTimeout = 10000)
        {
            errors.Add(new ErrorMessage()
            {
                Expire = DateTime.UtcNow.AddMilliseconds(msTimeout),
                Message = message
            });

            ErrorUpdated?.Invoke(this, new EventArgs());
        }

        public void ExpireErrors()
        {
            var count = errors.Count;
            for (var i = 0; i < errors.Count; i++)
            {
                if (DateTime.UtcNow > errors[i].Expire && !errors[i].IsPinned)
                {
                    errors.RemoveAt(i);
                    i--;
                }
            }

            if (count != errors.Count)
            {
                ErrorUpdated?.Invoke(this, new EventArgs());
            }
        }

        internal void PinError(ErrorMessage error, bool isPinned)
        {
            error.IsPinned = isPinned;
        }

        internal void RemoveError(ErrorMessage error)
        {
            errors.Remove(error);
            ErrorUpdated?.Invoke(this, new EventArgs());
        }
    }
}
