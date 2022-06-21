using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class AnalysisViewModel : ViewModelBase
    {
        private readonly AccountsApi accountsApi;

        public AnalysisViewModel(AccountsApi accountsApi)
        {
            this.accountsApi = accountsApi;
        }

        public override async Task OnInitializedAsync()
        {
            var tasks = new Task[]
            {
                Task.Run(async () => Accounts = await accountsApi.GetAccountsAsync()),
                Task.Run(async () => Transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts))
            };

            await Task.WhenAll(tasks);
            IsLoaded = true;
            await base.OnInitializedAsync();
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
                    .GroupBy(e => new { e.Tag, Date = new DateTime(e.Date.Year, e.Date.Month, 1) })
                    .Select(g => new { g.Key.Tag, g.Key.Date, Amount = g.Sum(e => e.Amount) });


                var lastSixMonths = tagSameMonthAmount
                    .Select(e => e.Date)
                    .Distinct()
                    .OrderByDescending(e => e.Date)
                    .Take(6)
                    .ToList();

                var interestingTags = tagSameMonthAmount
                    .GroupBy(e => e.Tag)
                    .Where(g => lastSixMonths.All(e => g.Any(m => m.Date == e)))
                    .Select(g => g.Key)
                    .ToList();

                var tagAmount = tagSameMonthAmount
                    .Where(e => interestingTags.Any(t => t == e.Tag))
                    .GroupBy(e => e.Tag)
                    .Select(g => new { Tag = g.Key, Amount = g.Sum(e => e.Amount) / g.Count() });

                var averageMonthlySpendingByTag = tagAmount
                    .Select(e => new KeyValuePair<string, decimal>(e.Tag, e.Amount))
                    .OrderByDescending(e => e.Value)
                    .Take(10);

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
                    .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, 1))
                    .OrderByDescending(g => g.Key)
                    .Take(12)
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
                    .Where(e => e.Date.Month == month.Month && e.Date.Year == month.Year);

                var tagsAmount = thisMonth
                    .SelectMany(e => e.Tags
                        .Select(t => new { Tag = t, Amount = Math.Abs(e.Amount) }))
                    .GroupBy(e => e.Tag)
                    .Select(g => new KeyValuePair<string, decimal>(g.Key, g.Sum(e => e.Amount))).ToList();

                var untagged = new KeyValuePair<string, decimal>("_", thisMonth.Where(e => e.Tags.IsNullOrEmpty()).Sum(e => Math.Abs(e.Amount)));
                tagsAmount.Add(untagged);

                return tagsAmount.ToList();
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

        public async Task OnSelectedAccountsChangeAsync()
        {
            Transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts);
            RaisePropertyChanged(nameof(Transactions));
        }


        public List<Transaction> Transactions { get; set; }
        public IEnumerable<Transaction> TransactionsQuery => Transactions.IsNullOrEmpty()
            ? null
            : Transactions
                //.Where(e => e.Amount < 0)
                .Where(e => ExcludeTags.IsNullOrEmpty() || !e.Tags.Any(t => ExcludeTags.Any(e => t == e)));

        public List<string> ExcludeTags { get; set; } = new List<string>();
        public List<Account> Accounts { get; set; }
        public IEnumerable<Guid> SelectedAccounts { get; set; }
        public string SelectedMonth { get; set; }
    }
}
