using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GdeBabki.Client.Services
{
    public class AccountsApi : ApiBase
    {
        public event EventHandler AccountsUpdated;
        public event EventHandler BanksUpdated;

        public AccountsApi(GBHttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            var response = await httpClient.GetAsync("api/Accounts");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<List<Account>>();
            return model;
        }

        public async Task<Guid> UpsertAccountAsync(UpsertAccount account)
        {
            var response = await httpClient.PostAsJsonAsync("api/Accounts", account);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<Guid>();

            AccountsUpdated?.Invoke(this, EventArgs.Empty);
            return model;
        }

        public async Task DeleteAccountAsync(Guid accountId)
        {

            var response = await httpClient.DeleteAsync($"api/Accounts?id={accountId}");
            response.EnsureSuccessStatusCode();

            AccountsUpdated?.Invoke(this, EventArgs.Empty);
        }

        public async Task<List<Bank>> GetBanksAsync()
        {
            var response = await httpClient.GetAsync("api/Accounts/Banks");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<List<Bank>>();
            return model;
        }

        public async Task<Guid> UpsertBankAsync(Bank bank)
        {
            var response = await httpClient.PostAsJsonAsync("api/Accounts/Banks", bank);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<Guid>();

            BanksUpdated?.Invoke(this, EventArgs.Empty);
            return model;
        }

        public async Task DeleteBankAsync(Guid bankId)
        {
            var response = await httpClient.DeleteAsync($"api/Accounts/Banks?id={bankId}");
            response.EnsureSuccessStatusCode();

            BanksUpdated?.Invoke(this, EventArgs.Empty);
        }

    }
}
