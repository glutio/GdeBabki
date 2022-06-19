using GdeBabki.Client.Model;
using GdeBabki.Client.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class ErrorViewModel: ViewModelBase
    {
        private readonly ErrorService errorService;
        private Timer timer;
        public IEnumerable<ErrorMessage> Errors => errorService.Errors;

        bool isFrozen;
        public bool IsFrozen { get { return isFrozen; } set { isFrozen = value; if (!isFrozen) RaisePropertyChanged(); } }

        public ErrorViewModel(ErrorService errorService)
        {            
            this.errorService = errorService;
        }

        public void TimerElapsed(object state)
        {
            errorService.ExpireErrors();
        }

        public override void OnInitialized()
        {
            errorService.ErrorUpdated += ErrorService_ErrorUpdated;
            timer = new Timer(TimerElapsed, null, 0, 100);
        }

        protected override void OnDispose()
        {
            timer.Dispose();
            errorService.ErrorUpdated -= ErrorService_ErrorUpdated;
        }

        private void ErrorService_ErrorUpdated(object sender, EventArgs e)
        {
            if (!IsFrozen)
            {
                RaisePropertyChanged(nameof(Errors));
            }
        }

        public void RemoveError(ErrorMessage error)
        {
            IsFrozen = false;
            errorService.RemoveError(error);
        }

        public void PinError(ErrorMessage error, bool isPinned)
        {
            errorService.PinError(error, isPinned);
        }
    }
}
