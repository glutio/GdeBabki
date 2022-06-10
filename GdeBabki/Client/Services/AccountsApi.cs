using GdeBabki.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GdeBabki.Client.Services
{
    public class AccountsApi : ApiBase
    {
        public event EventHandler AccountsUpdated;
        public event EventHandler BanksUpdated;

        public AccountsApi(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public async Task<List<Transaction>> GetTransactionsAsync(IEnumerable<Guid> accountIds)
        {
            var httpClient = httpFactory.CreateClient();
            var queryString = accountIds == null || accountIds.Count() == 0 ? "" : "accountIds=" + string.Join("&accountIds=", accountIds);
            var response = await httpClient.GetAsync($"api/Accounts/Transactions?{queryString}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<List<Transaction>>();

            return model;
        }

        public async Task<Guid> UpsertTransactionAsync(Transaction account)
        {
            var httpClient = httpFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync("api/Accounts/Transaction", account);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<Guid>();

            AccountsUpdated?.Invoke(this, EventArgs.Empty);
            return model;
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            var httpClient = httpFactory.CreateClient();
            var response = await httpClient.GetAsync("api/Accounts");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<List<Account>>();

            return model;
        }

        public async Task<Guid> UpsertAccountAsync(UpsertAccount account)
        {
            var httpClient = httpFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync("api/Accounts", account);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<Guid>();

            AccountsUpdated?.Invoke(this, EventArgs.Empty);
            return model;
        }

        public async Task DeleteAccountAsync(Guid accountId)
        {
            var httpClient = httpFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"api/Accounts?id={accountId}");
            response.EnsureSuccessStatusCode();

            AccountsUpdated?.Invoke(this, EventArgs.Empty);
        }

        public async Task<List<Bank>> GetBanksAsync()
        {
            var httpClient = httpFactory.CreateClient();
            var response = await httpClient.GetAsync("api/Accounts/Banks");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<List<Bank>>();
            return model;
        }

        public async Task<Guid> UpsertBankAsync(Bank bank)
        {
            var httpClient = httpFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync("api/Accounts/Banks", bank);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<Guid>();

            BanksUpdated?.Invoke(this, EventArgs.Empty);
            return model;
        }

        public async Task DeleteBankAsync(Guid bankId)
        {
            var httpClient = httpFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"api/Accounts/Banks?id={bankId}");
            response.EnsureSuccessStatusCode();

            BanksUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
