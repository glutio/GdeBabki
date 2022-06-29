using GdeBabki.Client.Pages;
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
    public class ReviewViewModel : ViewModelBase, IDisposable
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
        public IList<Transaction> SelectedTransactions
        {
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

        public int CurrentPage { get; set; }

        public Dictionary<string, Review.DataGridColumnState> DataGridColumnState { get; private set; } 

        public List<string> TagsFilterValue { get; set; } = new List<string>();

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

        public bool IsFrozen { get; set; }

        public ReviewViewModel(AccountsApi accountsApi, TagsApi tagsApi)
        {
            this.accountsApi = accountsApi;
            this.tagsApi = tagsApi;
        }

        public override async Task OnInitializedAsync()
        {
            if (!IsLoaded)
            {
                var tasks = new Task[]
                {
                    Task.Run(async () => Accounts = await accountsApi.GetAccountsAsync()),
                    Task.Run(async () => Transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts))
                };

                await Task.WhenAll(tasks);

                accountsApi.AccountsUpdated += AccountsApi_AccountsUpdated;
                
                Reset();
                
                IsLoaded = true;
            }
        }

        private async void AccountsApi_AccountsUpdated(object sender, EventArgs e)
        {
            Accounts = await accountsApi.GetAccountsAsync();
            if (SelectedAccounts == null)
            {
                await OnSelectedAccountsChangedAsync();
            }
            else
            {
                var selectedChanged = !SelectedAccounts.All(e => Accounts.Any(a => a.Id == e));
                SelectedAccounts = SelectedAccounts.Where(e => Accounts.Any(a => a.Id == e));

                if (selectedChanged)
                {
                    await OnSelectedAccountsChangedAsync();
                }
            }
        }

        public override void Dispose()
        {
            accountsApi.AccountsUpdated -= AccountsApi_AccountsUpdated;
            base.Dispose();
        }

        public async Task OnSelectedAccountsChangedAsync()
        {
            Transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts);

            if (!SelectedTransactions.IsNullOrEmpty())
            {
                SelectedTransactions = SelectedTransactions.Intersect(Transactions).ToList();
            }

            IsFrozen = false;
            RaisePropertyChanged(nameof(Transactions));
        }

        public void Reset()
        {
            SelectedTransactions = null;
            IsFrozen = false;
            CurrentPage = 0;
            TagsFilterValue.Clear();
            DataGridColumnState = new Dictionary<string, Review.DataGridColumnState>()
            {
                { nameof(Transaction.Date), new Review.DataGridColumnState() { Width = 130, SortOrder = SortOrder.Descending }  },
                { nameof(Transaction.Description), new Review.DataGridColumnState() { FilterOperator = FilterOperator.Contains } },
                { nameof(Transaction.Amount), new Review.DataGridColumnState() { Width = 110 } },
                { nameof(Transaction.Tags), new Review.DataGridColumnState() { FilterOperator = FilterOperator.GreaterThan }  },
            };
        }

        public IQueryable<Transaction> AddTagsFilter(IQueryable<Transaction> query)
        {
            var filterOperator = DataGridColumnState[nameof(Transaction.Tags)].FilterOperator;
            var filterValue = (List<string>)DataGridColumnState[nameof(Transaction.Tags)].FilterValue;
            if (!filterValue.IsNullOrEmpty())
            {
                switch (filterOperator)
                {
                    case FilterOperator.Equals: // e.Tag equals A B
                        query = query.Where(e => !e.Tags.IsNullOrEmpty() && e.Tags.All(t => filterValue.Any(f => t == f)));
                        break;
                    case FilterOperator.NotEquals: // e.Tag not equals A B
                        query = query.Where(e => !e.Tags.IsNullOrEmpty() && filterValue.All(f => e.Tags.All(t => t != f)));
                        break;
                    case FilterOperator.IsNull: // e.Tag is null or includes A || B
                        query = query.Where(e => e.Tags.IsNullOrEmpty() || e.Tags.Any(t => filterValue.Any(f => f == t)));
                        break;
                    case FilterOperator.IsNotNull: // e.Tag includes A | B
                        query = query.Where(e => e.Tags.Any(t => filterValue.Any(f => f == t)));
                        break;
                    case FilterOperator.GreaterThan: // e.Tag includes A | B
                        query = query.Where(e => e.Tags.Any(t => filterValue.Any(f => f == t)));
                        break;
                    case FilterOperator.GreaterThanOrEquals: // e.Tag includes A && B
                        query = query.Where(e => filterValue.All(f => e.Tags.Any(t => t == f)));
                        break;
                    case FilterOperator.LessThan: // e.Tag excludes A | B
                        query = query.Where(e => filterValue.Any(t => !e.Tags.Any(f => f == t)));
                        break;
                    case FilterOperator.LessThanOrEquals: // e.Tag excludes A & B
                        query = query.Where(e => filterValue.All(f => !e.Tags.Any(t => t == f)));
                        break;
                    default: // e.Tag includes A | B
                        query = query.Where(e => e.Tags.Any(t => filterValue.Any(f => f == t)));
                        break;
                }
            }
            else
            {
                switch (filterOperator)
                {
                    case FilterOperator.IsNotNull:
                        query = query.Where(e => !e.Tags.IsNullOrEmpty());
                        break;
                    case FilterOperator.IsNull:
                        query = query.Where(e => e.Tags.IsNullOrEmpty());
                        break;
                }

            }

            return query;
        }

        public void LoadData(LoadDataArgs args)
        {
            if (!IsFrozen)
            {
                var query = Transactions.AsQueryable();
                if (!string.IsNullOrEmpty(args.Filter))
                {
                    query = query.Where(args.Filter);
                }

                query = AddTagsFilter(query);

                if (!string.IsNullOrEmpty(args.OrderBy))
                {
                    query = query.OrderBy(args.OrderBy);
                }

                TransactionsQuery = query.ToList();
                TransactionsCount = query.Count();

                if (!SelectedTransactions.IsNullOrEmpty())
                {
                    SelectedTransactions = SelectedTransactions.Intersect(TransactionsQuery).ToList();
                }
            }

            var pageMax = (int)Math.Ceiling((double)TransactionsCount / args.Top.Value);
            args.Skip = Math.Min(args.Top.Value * (pageMax - 1), args.Skip.Value);

            TransactionsView = TransactionsQuery.Skip(args.Skip.Value).Take(args.Top.Value).ToList();

            IsFrozen = true;
        }

        public async Task DeleteSelectedTransactionsAsync()
        {
            if (SelectedTransactions.IsNullOrEmpty())
            {
                return;
            }

            try
            {
                IsBusy = true;
                await accountsApi.DeleteTrasactionsAsync(SelectedTransactions.Select(e => e.Id));
                foreach (var transaction in SelectedTransactions)
                {
                    Transactions.Remove(transaction);
                }
                SelectedTransactions = null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SaveTagAsync(string tag, Guid transactionId)
        {
            try
            {
                IsBusy = true;
                await tagsApi.AddTagAsync(new TransactionTag()
                {
                    TagId = tag,
                    TransactionId = transactionId
                });
            }
            finally
            {
                IsBusy = false;
            }
        }


        public async Task DeleteTagAsync(string tag, Guid transactionId)
        {
            try
            {
                IsBusy = true;
                await tagsApi.DeleteTagAsync(tag, transactionId);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SaveSharedTagAsync(string tag)
        {
            try
            {
                IsBusy = true;

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
            finally
            {
                IsBusy = false;
            }
        }

        public async Task DeleteSharedTagAsync(string tag)
        {
            try
            {
                IsBusy = true;

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
            finally
            {
                IsBusy = false;
            }
        }
    }
}
