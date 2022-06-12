using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class ReviewViewModel : ViewModelBase
    {
        private readonly AccountsApi accountsApi;
        private readonly TagsApi tagsApi;

        public List<Transaction> Transactions { get; set; }
        public List<Account> Accounts { get; set; }
        public IEnumerable<Guid> SelectedAccounts { get; set; }
        public List<string> FilterTags { get; set; }

        public ReviewViewModel(AccountsApi accountsApi, TagsApi tagsApi)
        {
            this.accountsApi = accountsApi;
            this.tagsApi = tagsApi;
        }

        public override async Task InitializeAsync()
        {
            Accounts = await accountsApi.GetAccountsAsync();
            if (SelectedAccounts == null)
            {
                SelectedAccounts = Accounts.Select(e => e.Id).ToArray();
            }
            
            Transactions = await GetTransactionsAsync();
            IsLoaded = true;

        }

        private async Task<List<Transaction>> GetTransactionsAsync()
        {
            var transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts);
            if (FilterTags != null && FilterTags.Count > 0)
            {
                Console.WriteLine("Hello");
                transactions = transactions.Where(e => e.Tags.Any(t => FilterTags.Any(f => f == t))).ToList();
            }

            return transactions;
        }

        public async Task OnSelectedAccountsChangeAsync()
        {
            Transactions = await GetTransactionsAsync();
            RaisePropertyChanged(nameof(Transactions));
        }

        public async Task AddTagAsync(string tag, Guid transactionId)
        {
            await tagsApi.InsertTagAsync(new TransactionTag()
            {
                TagId = tag,
                TransactionId = transactionId
            });
        }

        public async Task DeleteTagAsync(string tag, Guid transactionId)
        {
            await tagsApi.DeleteTagAsync(tag, transactionId);
        }

        public async Task SetFilterTagsAsync(List<string> tags)
        {
            FilterTags = tags;
            Transactions = await GetTransactionsAsync();
            RaisePropertyChanged(nameof(Transactions));
        }
    }
}
