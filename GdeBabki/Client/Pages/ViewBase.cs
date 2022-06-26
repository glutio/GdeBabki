using GdeBabki.Client.ViewModel;
using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GdeBabki.Client.Pages
{
    public class ViewBase<TModel> : ComponentBase, IDisposable where TModel : ViewModelBase
    {
        [Inject]
        public TModel Model { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Model.PropertyChanged += Model_PropertyChanged;
            await Model.OnInitializedAsync();
            await base.OnInitializedAsync();
        }

        public virtual void Dispose()
        {
            Model.PropertyChanged -= Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender, e);
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
