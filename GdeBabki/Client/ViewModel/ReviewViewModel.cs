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

        public List<Transaction> Transactions { get; private set; }
        public List<Transaction> TransactionsQuery { get; private set; }
        public List<Transaction> TransactionsView { get; private set; }
        public int TransactionsCount { get; set; }

        public IList<Transaction> SelectedTransactions { get; set; }
        public IList<Transaction> ActiveTransactions => SelectedTransactions.IsNullOrEmpty() ? TransactionsQuery : SelectedTransactions;

        public List<string> FilterTags { get; set; } = new List<string>();
        public FilterOperator TagsFilterOperator { get; set; }
        public List<string> SharedTags { get; set; }
        public bool IsUpdatingSharedTags { get; set; }

        public bool IsFrozen { get; set; }

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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("load data");
            if (!IsFrozen)
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
                            query = query.Where(e => !e.Tags.IsNullOrEmpty() && FilterTags.All(f => e.Tags.All(t => t != f)));
                            break;
                        case FilterOperator.IsNull:
                            query = query.Where(e => e.Tags.IsNullOrEmpty() || e.Tags.Any(t => FilterTags.Any(f => f == t)));
                            break;
                        default:
                            query = query.Where(e => e.Tags.Any(t => FilterTags.Any(f => f == t)));
                            break;
                    }
                }
                else 
                {
                    switch (TagsFilterOperator)
                    {
                        case FilterOperator.IsNotNull:
                            query = query.Where(e => !e.Tags.IsNullOrEmpty());
                            break;
                        case FilterOperator.IsNull:
                            query = query.Where(e => e.Tags.IsNullOrEmpty());
                            break;
                    }

                }

                if (!string.IsNullOrEmpty(args.OrderBy))
                {
                    query = query.OrderBy(args.OrderBy);
                }

                TransactionsQuery = query.ToList();
                TransactionsCount = query.Count();
            }

            TransactionsView = TransactionsQuery.Skip(args.Skip.Value).Take(args.Top.Value).ToList();

            UpdateSharedTags();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public void UpdateSharedTags(Transaction transaction = null)
        {
            var query = ActiveTransactions.AsQueryable();
            if (transaction != null)
            {
                if (SelectedTransactions == null)
                {
                    query = (new Transaction[] { transaction }).AsQueryable();
                }
                else
                {
                    if (SelectedTransactions.Contains(transaction))
                    {
                        query = query.Where(e => e.Id != transaction.Id);
                    }
                    else
                    {
                        query = query.Union(new Transaction[] { transaction });
                    }
                }
            }
            var allTags = query.SelectMany(e => e.Tags).Distinct().ToList();

            SharedTags = allTags.Where(tag => query.All(tran => tran.Tags.Any(e => e == tag))).ToList();
            RaisePropertyChanged(nameof(SharedTags));
        }

        private async Task<List<Transaction>> GetTransactionsAsync()
        {
            var transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts);
            return transactions;
        }

        public async Task OnSelectedAccountsChangeAsync()
        {
            IsFrozen = false;
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
            IsFrozen = true;

            var transactions = ActiveTransactions.Where(e => !e.Tags.Any(t => t == tag));

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
            IsFrozen = true;

            var transactions = ActiveTransactions.Where(e => e.Tags.Any(t => t == tag));

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
