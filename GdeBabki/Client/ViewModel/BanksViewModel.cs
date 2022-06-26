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
        private readonly ErrorService errorService;

        public BanksViewModel(AccountsApi accountsApi, ErrorService errorService)
        {
            this.accountsApi = accountsApi;
            this.errorService = errorService;
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
            await accountsApi.UpsertBankAsync(bank);
        }

        public async Task DeleteBankAsync(Guid id)
        {
            await accountsApi.DeleteBankAsync(id);
        }

        public bool Validate(string bankName)
        {
            if (string.IsNullOrWhiteSpace(bankName))
            {
                errorService.AddWarning("Please provide a bank name");
                return false;
            }

            return true;
        }
        public List<Bank> Banks { get; set; }
    }
}
