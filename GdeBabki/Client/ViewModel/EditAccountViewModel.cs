using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class EditAccountViewModel: ViewModelBase, IDisposable
    {
        private readonly AccountsApi accountsApi;

        public EditAccountViewModel(AccountsApi accountsApi)
        {
            this.accountsApi = accountsApi;
        }

        public override void Dispose()
        {
            accountsApi.BanksUpdated -= AccountsApi_BanksUpdated;
            base.Dispose();
        }

        private async void AccountsApi_BanksUpdated(object sender, EventArgs e)
        {
            Banks = await accountsApi.GetBanksAsync();
            RaisePropertyChanged(nameof(Banks));
        }

        public override async Task OnInitializedAsync()
        {
            accountsApi.BanksUpdated += AccountsApi_BanksUpdated;
            Banks = await accountsApi.GetBanksAsync();
            IsLoaded = true;
        }

        public Account Account { get; set; }
        public List<Bank> Banks { get; set; }
    }
}
