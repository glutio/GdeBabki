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


        IList<Transaction> selectedTransactions;
        public IList<Transaction> SelectedTransactions { 
            get 
            { 
                return selectedTransactions;  
            } 
            set 
            { 
                if (value != selectedTransactions) 
                { 
                    selectedTransactions = value;
                    RaisePropertyChanged();
                } 
            } 
        }
        public IList<Transaction> ActiveTransactions => SelectedTransactions.IsNullOrEmpty() ? TransactionsQuery : SelectedTransactions;

        public List<string> FilterTags { get; set; } = new List<string>();
        public FilterOperator TagsFilterOperator { get; set; }
        public List<string> SharedTags
        {
            get
            {
                if (ActiveTransactions == null)
                    return null;

                var sharedTags = ActiveTransactions
                    .SelectMany(e => e.Tags)
                    .Distinct()
                    .Where(tag => 
                        ActiveTransactions.All(tran => 
                            tran.Tags.Any(e => e == tag)
                     ))
                    .ToList();

                return sharedTags;
            }
        }
        public bool IsUpdatingSharedTags { get; set; }

        public bool IsFrozen { get; set; }

        public ReviewViewModel(AccountsApi accountsApi, TagsApi tagsApi)
        {
            this.accountsApi = accountsApi;
            this.tagsApi = tagsApi;
        }

        public override async Task OnInitializeAsync()
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
                        case FilterOperator.Equals: // e.Tag equals A B
                            query = query.Where(e => !e.Tags.IsNullOrEmpty() && e.Tags.All(t => FilterTags.Any(f => t == f)));
                            break;
                        case FilterOperator.NotEquals: // e.Tag not equals A B
                            query = query.Where(e => !e.Tags.IsNullOrEmpty() && FilterTags.All(f => e.Tags.All(t => t != f)));
                            break;
                        case FilterOperator.IsNull: // e.Tag is null or includes A || B
                            query = query.Where(e => e.Tags.IsNullOrEmpty() || e.Tags.Any(t => FilterTags.Any(f => f == t)));
                            break;
                        case FilterOperator.IsNotNull: // e.Tag includes A | B
                            query = query.Where(e => e.Tags.Any(t => FilterTags.Any(f => f == t)));
                            break;
                        case FilterOperator.GreaterThan: // e.Tag includes A | B
                            query = query.Where(e => e.Tags.Any(t => FilterTags.Any(f => f == t)));
                            break;
                        case FilterOperator.GreaterThanOrEquals: // e.Tag includes A && B
                            query = query.Where(e => FilterTags.All(f => e.Tags.Any(t => t == f)));
                            break;
                        case FilterOperator.LessThan: // e.Tag excludes A | B
                            query = query.Where(e => FilterTags.Any(t => !e.Tags.Any(f => f == t)));
                            break;
                        case FilterOperator.LessThanOrEquals: // e.Tag excludes A & B
                            query = query.Where(e => FilterTags.All(f => !e.Tags.Any(t => t == f)));
                            break;
                        default: // e.Tag includes A | B
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

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            IsFrozen = true;
        }

        private async Task<List<Transaction>> GetTransactionsAsync()
        {
            var transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts);
            return transactions;
        }

        public async Task OnSelectedAccountsChangeAsync()
        {
            SelectedTransactions = null;
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
