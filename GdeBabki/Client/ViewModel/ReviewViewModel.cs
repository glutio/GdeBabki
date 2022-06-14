using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using Radzen;
using System;
using System.Collections.Generic;
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

        public List<Transaction> Transactions { get; private set; }
        public IQueryable<Transaction> TransactionsQuery { get; private set; }
        public IQueryable<Transaction> TransactionsView { get; private set; }
        public int TransactionsCount { get; set; }

        public List<string> FilterTags { get; set; } = new List<string>();
        public FilterOperator TagsFilterOperator { get; set; }
        public List<string> SharedTags { get; set; }
        public bool IsUpdatingSharedTags { get; set; }

        public bool Freeze { get; set; }

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
            Console.WriteLine("load data");
            if (!Freeze)
            {
                var query = Transactions.AsQueryable();
                if (!string.IsNullOrEmpty(args.Filter))
                {
                    query = query.Where(args.Filter);
                }

                if (FilterTags.Count > 0)
                {
                    switch (TagsFilterOperator)
                    {
                        case FilterOperator.Equals:
                            query = query.Where(e => !e.Tags.IsNullOrEmpty() && e.Tags.All(t => FilterTags.Any(f => t == f)));
                            break;
                        case FilterOperator.NotEquals:
                            query = query.Where(e => FilterTags.All(f => e.Tags.All(t => t != f)));
                            break;
                        case FilterOperator.IsNull:
                            query = query.Where(e => e.Tags.IsNullOrEmpty() || e.Tags.Any(t => FilterTags.Any(f => f == t)));
                            break;
                        default:
                            query = query.Where(e => e.Tags.Any(t => FilterTags.Any(f => f == t)));
                            break;
                    }
                }
                else if (TagsFilterOperator == FilterOperator.IsNull)
                {
                    query = query.Where(e => e.Tags.IsNullOrEmpty());
                }

                if (!string.IsNullOrEmpty(args.OrderBy))
                {
                    query = query.OrderBy(args.OrderBy);
                }

                TransactionsQuery = query;
                TransactionsCount = query.Count();
            }

            UpdateSharedTags();

            TransactionsView = TransactionsQuery.Skip(args.Skip.Value).Take(args.Top.Value);
        }

        public void UpdateSharedTags()
        {
            var allTags = TransactionsQuery.SelectMany(e => e.Tags).Distinct().ToList();
            SharedTags = allTags.Where(tag => TransactionsQuery.All(tran => tran.Tags.Any(e => e == tag))).ToList();
            RaisePropertyChanged(nameof(SharedTags));
        }

        private async Task<List<Transaction>> GetTransactionsAsync()
        {
            var transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts);
            return transactions;
        }

        public async Task OnSelectedAccountsChangeAsync()
        {
            Freeze = false;
            Transactions = await GetTransactionsAsync();
            RaisePropertyChanged(nameof(Transactions));
        }

        public async Task SaveTagAsync(string tag, Guid transactionId)
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
            Freeze = true;

            var transactions = TransactionsQuery.Where(e => !e.Tags.Any(t => t == tag));

            await tagsApi.AddSharedTagAsync(new SharedTag()
            {
                TagId = tag,
                TransactionIds = transactions.Select(e => e.Id).ToList()
            });

            foreach (var transaction in transactions)
            {
                transaction.Tags.Add(tag);
            }
        }

        public async Task DeleteSharedTagAsync(string tag)
        {
            Freeze = true;

            var transactions = TransactionsQuery.Where(e => e.Tags.Any(t => t == tag));

            await tagsApi.DeleteSharedTagsAsync(new SharedTag()
            {
                TagId = tag,
                TransactionIds = transactions.Select(e => e.Id).ToList()
            });

            foreach (var transaction in transactions)
            {
                transaction.Tags.Remove(tag);
            }
        }
    }
}
