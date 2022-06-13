using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using Radzen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class ReviewViewModel : ViewModelBase
    {
        private readonly AccountsApi accountsApi;
        private readonly TagsApi tagsApi;

        public List<Account> Accounts { get; set; }
        public IEnumerable<Guid> SelectedAccounts { get; set; }

        public List<Transaction> Transactions { get; set; }
        public IQueryable<Transaction> TransactionsQuery { get; private set; }
        public List<Transaction> TransactionsView { get; set; }
        public int TransactionsCount => TransactionsQuery?.Count() ?? 0;

        public List<string> FilterTags { get; set; } = new List<string>();
        public List<string> SharedTags { get; set; }
        public bool IsUpdatingSharedTags { get; set; }

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

        public void LoadData(LoadDataArgs args)
        {
            var query = Transactions.AsQueryable();
            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (FilterTags.Count > 0)
            {
                query = query.Where(e => e.Tags.Any(t => FilterTags.Any(f => f == t)));
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            TransactionsQuery = query;
            TransactionsView = query.Skip(args.Skip.Value).Take(args.Top.Value).ToList();

            if (!IsUpdatingSharedTags)
            {
                var allTags = query.SelectMany(e => e.Tags).Distinct().ToList();
                SharedTags = allTags.Where(tag => query.All(tran => tran.Tags.Any(e => e == tag))).ToList();
            }
        }

        private async Task<List<Transaction>> GetTransactionsAsync()
        {
            var transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts);
            return transactions;
        }

        public async Task OnSelectedAccountsChangeAsync()
        {
            Transactions = await GetTransactionsAsync();
            RaisePropertyChanged(nameof(Transactions));
        }

        public async Task AddTagAsync(string tag, Guid transactionId)
        {
            await tagsApi.AddTagAsync(new TransactionTag()
            {
                TagId = tag,
                TransactionId = transactionId
            });
        }

        public async Task DeleteTagAsync(string tag, Guid transactionId)
        {
            await tagsApi.DeleteTagAsync(tag, transactionId);
        }

        public async Task SaveSharedTagAsync(string tag)
        {
            var transactions = TransactionsQuery.Where(e => !e.Tags.Any(t => t == tag));

            await tagsApi.AddTagsAsync(transactions
                .Select(e => new TransactionTag()
                {
                    TagId = tag,
                    TransactionId = e.Id
                }));

            foreach(var transaction in transactions)
            {
                transaction.Tags.Add(tag);
            }
        }

        public async Task DeleteSharedTagAsync(string tag)
        {
            var transactions = TransactionsQuery.Where(e => e.Tags.Any(t => t == tag));

            await tagsApi.DeleteTagsAsync(tag, transactions
                .Select(e => e.Id));

            foreach (var transaction in transactions)
            {
                transaction.Tags.Remove(tag);
            }
        }
    }
}
