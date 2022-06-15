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
            Transactions = await accountsApi.GetTransactionsAsync(null);
            Console.WriteLine(Transactions.Count());
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

                var all = new List<KeyValuePair<string, Transaction>>();
                var averageMonthlySpendingByTag = Transactions.Where(e=>e.Amount < 0)
                    .SelectMany(e => e.Tags
                        .Select(t => new { Tag = t, e.Date, e.Amount }))
                    .GroupBy(e => new { e.Tag, Date = new DateTime(e.Date.Year, e.Date.Month, 1) })
                    .Select(g => new { g.Key.Tag, g.Key.Date.Month, Amount = g.Sum(e => e.Amount) })
                    .GroupBy(e => new { e.Tag, e.Month })
                    .Select(g => new { g.Key.Tag, Amount = g.Sum(e => e.Amount), Count = g.Count() })
                    .GroupBy(e => e.Tag)
                    .Select(g => new { Tag = g.Key, Amount = g.Sum(e => e.Amount), Count = g.Count() })
                    .Select(e => new KeyValuePair<string, decimal>(e.Tag, Math.Round(e.Amount / e.Count, 2)))
                    .OrderByDescending(e => e.Value)
                    .Take(7);


                //var averageMonthlySpendingByTag = all
                //    .GroupBy(e => new { Tag = e.Key, Date =  })
                //    .Select(g => new { Tag = g.Key.Tag, Sum = g.Sum(e => e.Value.Amount), Count = g.Count() })
                //    .GroupBy(e => e.Tag)
                //    .Select(g => new KeyValuePair<string, decimal>(g.Key, Math.Round(g.Average(e => e.Sum / e.Count), 2)))
                //    .OrderByDescending(e => e.Value)
                //    .Take(7);

                return averageMonthlySpendingByTag.ToList();
            }
        }

        public List<Transaction> Transactions { get; set; }
        public List<Transaction> TransactionsQuery => Transactions;
    }
}
