using GdeBabki.Client.Model;
using GdeBabki.Client.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class ErrorViewModel: ViewModelBase
    {
        private readonly ErrorService errorService;

        public IEnumerable<ErrorMessage> Errors => errorService.Errors;

        bool isFrozen;
        public bool IsFrozen { get { return isFrozen; } set { isFrozen = value; if (!isFrozen) RaisePropertyChanged(); } }

        public ErrorViewModel(ErrorService errorService)
        {            
            this.errorService = errorService;
        }

        public override void Initialize()
        {
            errorService.ErrorUpdated += ErrorService_ErrorUpdated;
        }

        protected override void OnDispose()
        {
            errorService.ErrorUpdated -= ErrorService_ErrorUpdated;
            errorService.Dispose();
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
            errorService.RemoveError(error);
        }

        public void PinError(ErrorMessage error, bool isPinned)
        {
            errorService.PinError(error, isPinned);
        }


    }
}
