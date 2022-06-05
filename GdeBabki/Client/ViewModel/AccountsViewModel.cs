using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class AccountsViewModel: ViewModelBase
    {
        private readonly AccountsApi accountsApi;

        public AccountsViewModel(AccountsApi accountsApi)
        {
            this.accountsApi = accountsApi;
        }

        public async Task InitializeAsync()
        {
            accountsApi.AccountsUpdated += AccountsApi_AccountsUpdated;
            Accounts = await accountsApi.GetAccountsAsync();
            RaisePropertyChanged(nameof(Accounts));
        }

        private async void AccountsApi_AccountsUpdated(object sender, EventArgs e)
        {
            Accounts = await accountsApi.GetAccountsAsync();
            RaisePropertyChanged(nameof(Accounts));
        }

        public void NewAccount()
        {
            if (Accounts.Count > 0 && Accounts[Accounts.Count - 1].Id == Guid.Empty)
                return;

            Accounts.Add(new Account());
            RaisePropertyChanged(nameof(Accounts));
        }

        public List<Account> Accounts { get; set; }
    }
}
