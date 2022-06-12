using System.Collections.Generic;

namespace GdeBabki.Client.ViewModel
{
    public class PopupViewModel: ViewModelBase
    {
        bool isOpen;
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                if (value != isOpen)
                {
                    isOpen = value; 
                    RaisePropertyChanged();
                }
            }
        }
    }
}
