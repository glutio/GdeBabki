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

        public async Task<Guid> AddAccountAsync(AddAccount account)
        {
            var response = await httpClient.PostAsJsonAsync("api/Accounts", account);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<Guid>();
            
            AccountsUpdated?.Invoke(this, EventArgs.Empty);
            return model;
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            var response = await httpClient.GetAsync("api/Accounts");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<List<Account>>();
            return model;
        }

        internal async Task UpdateBankAsync(Bank addBank)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> AddBankAsync(AddBank bank)
        {
            var response = await httpClient.PostAsJsonAsync("api/Accounts/Bank", bank);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<Guid>();

            BanksUpdated?.Invoke(this, EventArgs.Empty);
            return model;
        }

        public async Task<List<Bank>> GetBanksAsync()
        {
            var response = await httpClient.GetAsync("api/Accounts/Banks");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<List<Bank>>();
            return model;
        }
    }
}
