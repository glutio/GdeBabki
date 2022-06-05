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
            this.accountsApi.BanksUpdated += AccountsApi_BanksUpdated;
        }

        public void Dispose()
        {
            this.accountsApi.BanksUpdated -= AccountsApi_BanksUpdated;
        }

        private async void AccountsApi_BanksUpdated(object sender, EventArgs e)
        {
            Banks = await accountsApi.GetBanksAsync();

            RaisePropertyChanged(nameof(Banks));
        }

        public async Task InitializeAsync()
        {
            Banks = await accountsApi.GetBanksAsync();            
        }

        public async Task<Guid> Save()
        {
            return await accountsApi.UpsertAccountAsync(new UpsertAccount()
            {
                AccountId = AccountId,
                Name = Name,
                BankId = BankId
            });
        }

        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public Guid BankId { get; set; }
        public List<Bank> Banks { get; set; }
    }
}
