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
        protected TModel Model { get; set; }

        protected override void OnInitialized()
        {
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }

        protected async override Task OnInitializedAsync()
        {
            await Model.InitializeAsync();
        }

        public void Dispose()
        {
            Model.PropertyChanged -= OnPropertyChanged;
            Model.Dispose();
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
