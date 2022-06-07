using GdeBabki.Client.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        protected virtual void Unsubscribe()
        {
        }

        bool isLoaded;
        public bool IsLoaded { get { return isLoaded; } set { isLoaded = value; RaisePropertyChanged(); } }

    }
}
