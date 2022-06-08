using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class ReviewViewModel : ViewModelBase
    {
        private readonly AccountsApi accountsApi;

        public List<Transaction> Transactions { get; set; }
        public List<KeyValuePair<Account, bool>> Accounts { get; set; }

        public ReviewViewModel(AccountsApi accountsApi)
        {
            this.accountsApi = accountsApi;
        }

        public override async Task InitializeAsync()
        {
            var accounts = await accountsApi.GetAccountsAsync();
            Accounts = accounts.Select(e => new KeyValuePair<Account, bool>(e, true)).ToList();
            
            Transactions = await accountsApi.GetTransactionsAsync(accounts.ToArray());
            IsLoaded = true;
        }
    }
}
