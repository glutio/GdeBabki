using GdeBabki.Client.Services;
using System;

namespace GdeBabki.Client.Model
{
    public class ErrorMessage
    {
        public bool IsPinned { get; set; }
        public DateTime Expire { get; set; }
        public string Message { get; set; }
        public ErrorService.NotificationType NotificationType { get; set; }
    }
}
