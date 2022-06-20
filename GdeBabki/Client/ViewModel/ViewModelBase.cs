﻿using GdeBabki.Client.Services;
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

        public virtual void OnInitialized()
        {
        }

        public virtual Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }

        bool isLoaded;
        public bool IsLoaded { get { return isLoaded; } set { isLoaded = value; RaisePropertyChanged(); } }

        bool isBusy;
        public bool IsBusy { get { return isBusy; } set { isBusy = value; RaisePropertyChanged(); } }
    }
}
