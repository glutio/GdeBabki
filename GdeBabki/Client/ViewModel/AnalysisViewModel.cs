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

        public override async Task OnInitializeAsync()
        {
            var tasks = new Task[]
            { 
                Task.Run(async () => Accounts = await accountsApi.GetAccountsAsync()),
                Task.Run(async () => Transactions = await accountsApi.GetTransactionsAsync(SelectedAccounts))
            };

            await Task.WhenAll(tasks);
            IsLoaded = true;
            await base.OnInitializeAsync();
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
                        .Select(t => new { Tag = t, e.Date, Amount = -1 * e.Amount }));

                var tagSameMonthAmount = tagDateAmount
                    .GroupBy(e => new { e.Tag, Date = new DateTime(e.Date.Year, e.Date.Month, 1) })
                    .Select(g => new { g.Key.Tag, g.Key.Date, Amount = g.Sum(e => e.Amount) });

                var tagAmount = tagSameMonthAmount
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
                    .Take(9)
                    .Select(g => new KeyValuePair<string, decimal>(g.Key.ToTransactionMonthYear(), g.Sum(e => -1 * e.Amount)));


                return spendingByMonth.ToList();
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
                .Where(e => e.Amount < 0)
                .Where(e => ExcludeTags.IsNullOrEmpty() || !e.Tags.Any(t => ExcludeTags.Any(e => t == e)));

        public List<string> ExcludeTags { get; set; } = new List<string>();
        public List<Account> Accounts { get; set; }
        public IEnumerable<Guid> SelectedAccounts { get; set; }

    }
}
