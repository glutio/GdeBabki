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
        private readonly ErrorService errorService;

        public EditAccountViewModel(AccountsApi accountsApi, ErrorService errorService)
        {
            this.accountsApi = accountsApi;
            this.errorService = errorService;
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

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Account.Name))
            {
                errorService.AddWarning("Please provide account name");
                return false;
            }
            if (Account.Bank.Id == Guid.Empty)
            {
                errorService.AddWarning("Please select a bank");
                return false;
            }

            return true;
        }

        public Account Account { get; set; }
        public List<Bank> Banks { get; set; }
    }
}
