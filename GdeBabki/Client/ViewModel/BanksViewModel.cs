using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class BanksViewModel: ViewModelBase
    {
        private readonly AccountsApi accountsApi;

        public BanksViewModel(AccountsApi accountsApi)
        {
            this.accountsApi = accountsApi;
            accountsApi.BanksUpdated += AccountsApi_BanksUpdated;
        }

        private async void AccountsApi_BanksUpdated(object sender, EventArgs e)
        {
            Banks = await accountsApi.GetBanksAsync();
            RaisePropertyChanged(nameof(Banks));
        }

        public override void Dispose()
        {
            accountsApi.BanksUpdated -= AccountsApi_BanksUpdated;
            base.Dispose();
        }

        public override async Task OnInitializedAsync()
        {
            Banks = await accountsApi.GetBanksAsync();
            IsLoaded = true;
        }

        public async Task UpsertBankAsync(Bank bank)
        {
            var id = await accountsApi.UpsertBankAsync(bank);
        }

        public async Task DeleteBankAsync(Guid id)
        {
            await accountsApi.DeleteBankAsync(id);
        }

        public List<Bank> Banks { get; set; }
    }
}
