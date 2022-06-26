using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class AccountsViewModel : ViewModelBase
    {
        private readonly AccountsApi accountsApi;

        public AccountsViewModel(AccountsApi accountsApi)
        {
            this.accountsApi = accountsApi;
        }

        public override async Task OnInitializedAsync()
        {
            accountsApi.AccountsUpdated += AccountsApi_AccountsUpdated;
            Accounts = await accountsApi.GetAccountsAsync();
            IsLoaded = true;
        }

        public override void Dispose()
        {
            accountsApi.AccountsUpdated -= AccountsApi_AccountsUpdated;
            base.Dispose();
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

            var account = new Account()
            {
                Bank = new Bank()
            };

            Accounts.Add(account);
            EditingAccount = account;
            RaisePropertyChanged(nameof(EditingAccount));
        }

        public void EditAccount(Account account)
        {
            CancelEditingAccount();
            EditingAccount = account;
            RaisePropertyChanged(nameof(EditingAccount));
        }

        public void CancelEditingAccount()
        {
            if (EditingAccount == null)
                return;

            if (EditingAccount.Id == Guid.Empty)
                Accounts.RemoveAt(Accounts.Count - 1);

            EditingAccount = null;
            RaisePropertyChanged(nameof(EditingAccount));
        }

        public async Task DeleteAccountAsync(Guid accountId)
        {
            await accountsApi.DeleteAccountAsync(accountId);
        }

        public async Task SaveAccountAsync(Account account)
        {
            await accountsApi.UpsertAccountAsync(new UpsertAccount()
            {
                AccountId = account.Id,
                Name = account.Name,
                BankId = account.Bank.Id
            });
            EditingAccount = null;
            RaisePropertyChanged(nameof(EditingAccount));
        }

        public Account EditingAccount { get; set; }
        public List<Account> Accounts { get; set; }
    }
}
