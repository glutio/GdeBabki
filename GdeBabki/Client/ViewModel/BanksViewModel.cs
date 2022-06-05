using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class BanksViewModel: ViewModelBase
    {
        private readonly AccountsApi accountsApi;

        public BanksViewModel(AccountsApi accountsApi)
        {
            this.accountsApi = accountsApi;
        }

        public async Task Initialize()
        {
            Banks = await accountsApi.GetBanksAsync();
        }

        public async Task AddBankAsync(string Name)
        {
            var id = await accountsApi.AddBankAsync(new AddBank()
            {
                Name = Name
            });

            Banks.Add(new Bank() { Id = id, Name = Name });
            RaisePropertyChanged(nameof(Banks));
        }

        public async Task UpdateBankAsync(Guid id, string Name)
        {

        }

        public List<Bank> Banks { get; set; }
    }
}
