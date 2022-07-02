using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class AnalysisViewModel : ViewModelBase
    {
        private readonly AccountsApi accountsApi;

        public AnalysisViewModel(AccountsApi accountsApi)
        {
            this.accountsApi = accountsApi;
            SelectedDateRange = DateRange[0].Value;
        }

        public override async Task OnInitializedAsync()
        {
            if (!IsLoaded)
            {
                accountsApi.AccountsUpdated += AccountsApi_AccountsUpdated;

                await Task.WhenAll(new Task[] {
                    Task.Run(async () => Transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts)),
                    Task.Run(async () => Accounts = await accountsApi.GetAccountsAsync()),
                });

                IsLoaded = true;
            }

            await base.OnInitializedAsync();
        }

        private async void AccountsApi_AccountsUpdated(object sender, System.EventArgs e)
        {
            Accounts = await accountsApi.GetAccountsAsync();

            bool selectedChanged = true;
            if (SelectedAccounts != null)
            {
                selectedChanged = !SelectedAccounts.All(e => Accounts.Any(a => a.Id == e));
                SelectedAccounts = SelectedAccounts.Where(e => Accounts.Any(a => a.Id == e)).ToList();
            }

            if (selectedChanged)
            {
                await OnSelectedAccountsChangedAsync();
            }
        }

        public override void Dispose()
        {
            accountsApi.AccountsUpdated -= AccountsApi_AccountsUpdated;
            base.Dispose();
        }

        public List<KeyValuePair<string, decimal>> AverageMonthlySpendingByTag
        {
            get
            {
                if (Transactions.IsNullOrEmpty())
                {
                    return null;
                }

                var tagDateAmount = TransactionsQuery
                    .SelectMany(e => e.Tags
                        .Select(t => new { Tag = t, e.Date, Amount = Math.Abs(e.Amount) }));

                var tagSameMonthAmount = tagDateAmount
                    .GroupBy(e => new { e.Tag, Date = e.Date.ToMonth() })
                    .Select(g => new { g.Key.Tag, g.Key.Date, Amount = g.Sum(e => e.Amount) });

                var allMonths = tagSameMonthAmount
                    .Select(e => e.Date)
                    .Distinct()
                    .OrderByDescending(e=> e.Date)
                    .Skip(1)
                    .ToList();

                var interestingTags = tagSameMonthAmount
                    .GroupBy(e => e.Tag)
                    .Where(g => allMonths.Sum(m => g.Any(t => t.Date == m) ? 1 : 0) >= allMonths.Count * 0.8)
                    .Select(g => g.Key)
                    .ToList();

                var tagAmount = tagSameMonthAmount
                    .Where(e => interestingTags.Any(t => t == e.Tag))
                    .GroupBy(e => e.Tag)
                    .Select(g => new { Tag = g.Key, Amount = g.Sum(e => e.Amount) / g.Count() });

                var averageMonthlySpendingByTag = tagAmount
                    .Select(e => new KeyValuePair<string, decimal>(e.Tag, e.Amount))
                    .OrderByDescending(e => e.Value);

                return averageMonthlySpendingByTag.ToList();
            }
        }

        public List<KeyValuePair<string, decimal>> SpendingByMonth
        {
            get
            {
                if (Transactions.IsNullOrEmpty())
                {
                    return null;
                }

                var spendingByMonth = TransactionsQuery
                    .GroupBy(e => e.Date.ToMonth())
                    .OrderBy(g => g.Key)
                    .Select(g => new KeyValuePair<string, decimal>(g.Key.ToTransactionMonthYear(), g.Sum(e => Math.Abs(e.Amount))));

                return spendingByMonth.ToList();
            }
        }

        public List<KeyValuePair<string, decimal>> SpendingByTagThisMonth
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedMonth))
                {
                    return null;
                }

                var month = DateTime.Parse(SelectedMonth);

                var thisMonth = TransactionsQuery
                    .Where(e => e.Date.Month == month.Month && e.Date.Year == month.Year)
                    .ToList();

                var tagsAmount = thisMonth
                    .SelectMany(e => e.Tags
                        .Select(t => new { Tag = t, Amount = Math.Abs(e.Amount) }))
                    .GroupBy(e => e.Tag)
                    .Select(g => new KeyValuePair<string, decimal>(g.Key, g.Sum(e => e.Amount)))
                    .ToList();

                if (!thisMonth.IsNullOrEmpty())
                {
                    var untagged = new KeyValuePair<string, decimal>("", thisMonth.Where(e => e.Tags.IsNullOrEmpty()).Sum(e => Math.Abs(e.Amount)));
                    if (untagged.Value != 0)
                    {
                        tagsAmount.Add(untagged);
                    }
                }

                return tagsAmount.OrderByDescending(e=>e.Value).ToList();
            }
        }

        public string[] GetTagsInMonth(DateTime date)
        {
            if (Transactions.IsNullOrEmpty())
            {
                return null;
            }

            return TransactionsQuery
                .Where(e => e.Date >= date && e.Date < e.Date.AddMonths(1))
                .SelectMany(e => e.Tags)
                .Distinct()
                .ToArray();
        }

        public async Task OnSelectedAccountsChangedAsync()
        {
            Transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts);
            RaisePropertyChanged(nameof(Transactions));
        }

        public List<Transaction> Transactions { get; set; }
        public IEnumerable<Transaction> TransactionsQuery
        {
            get
            {
                var query = Transactions.IsNullOrEmpty()
                    ? null
                    : Transactions
                        .Where(e => ExcludeTags.IsNullOrEmpty() || !e.Tags.Any(t => ExcludeTags.Any(e => t == e)));
                
                return SelectedDateRange(query);
            }
        }

        public List<string> ExcludeTags { get; set; } = new List<string>();
        public List<Account> Accounts { get; set; }
        public IEnumerable<Guid> SelectedAccounts { get; set; }
        public string SelectedMonth { get; set; }
        public DateRangeFilter SelectedDateRange { get; set; }

        public delegate IEnumerable<Transaction> DateRangeFilter (IEnumerable<Transaction> query);

        static IEnumerable<Transaction> RecentMonthsFilter(IEnumerable<Transaction> query, int months)
        {
            return query.GroupBy(e => e.Date.ToMonth()).OrderByDescending(g => g.Key).Take(months).SelectMany(g => g);
        }

        static IEnumerable<Transaction> YearToDateFilter(IEnumerable<Transaction> query)
        {
            return query.GroupBy(e => e.Date.ToYear()).OrderByDescending(g => g.Key).Take(1).SelectMany(g => g);
        }

        public List<KeyValuePair<string, DateRangeFilter>> DateRange { get; } = new List<KeyValuePair<string, DateRangeFilter>>()
        {
            new KeyValuePair<string, DateRangeFilter>("All", q => q),
            new KeyValuePair<string, DateRangeFilter>("3 months", q => RecentMonthsFilter(q, 3)),
            new KeyValuePair<string, DateRangeFilter>("6 months", q => RecentMonthsFilter(q, 6)),
            new KeyValuePair<string, DateRangeFilter>("9 months", q => RecentMonthsFilter(q, 9)),
            new KeyValuePair<string, DateRangeFilter>("12 months", q => RecentMonthsFilter(q, 12)),
            new KeyValuePair<string, DateRangeFilter>("Year to date", q => YearToDateFilter(q)),
        };
    }
}
