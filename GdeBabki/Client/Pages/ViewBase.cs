using GdeBabki.Client.ViewModel;
using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;

namespace GdeBabki.Client.Pages
{
    public class ViewBase<TModel> : ComponentBase, IDisposable where TModel : ViewModelBase
    {
        protected TModel Model { get; set; }

        public ViewBase(TModel model)
        {
            Model = model;
        }

        protected override void OnInitialized()
        {
            Model.PropertyChanged += OnPropertyChanged;
        }

        public void Dispose()
        {
            Model.PropertyChanged -= OnPropertyChanged;
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
