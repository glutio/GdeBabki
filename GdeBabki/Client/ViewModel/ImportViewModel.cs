using GdeBabki.Client.Services;
using GdeBabki.Shared;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class ImportViewModel : ViewModelBase
    {
        public const int MAX_STREAM_SIZE = 1024 * 1024 * 2; /* 2 mb */
        private readonly AccountsApi accountsApi;
        private readonly ImportApi importApi = null;
        private readonly ErrorService errorService;

        public ImportViewModel(AccountsApi accountsApi, ImportApi importApi, ErrorService errorService)
        {
            this.accountsApi = accountsApi;
            this.importApi = importApi;
            this.errorService = errorService;
        }

        public override async Task OnInitializedAsync()
        {
            accountsApi.AccountsUpdated += AccountsApi_AccountsUpdated;
            Accounts = await accountsApi.GetAccountsAsync();
            IsLoaded = true;
        }

        public override void Dispose()
        {
            accountsApi.AccountsUpdated -= AccountsApi_AccountsUpdated;
            base.Dispose();
        }

        private async void AccountsApi_AccountsUpdated(object sender, EventArgs e)
        {
            try
            {
                IsBusy = true;
                Accounts = await accountsApi.GetAccountsAsync();
            }
            finally
            {
                IsBusy = false;
            }

            RaisePropertyChanged(nameof(Accounts));
        }

        public async Task LoadSampleLinesAsync(int count)
        {
            SampleLines = null;
            ColumnMapping = null;

            using var stream = ImportFile.OpenReadStream(MAX_STREAM_SIZE);

            try
            {
                IsBusy = true;
                var parser = new CsvParser();
                SampleLines = await parser.LoadAsync(stream, count);

                if (SampleLines.Count > 0)
                {
                    ColumnMapping = new GBColumnName?[SampleLines.Select(e=>e.Length).Max()];
                }

                for(int i = 0; i < SampleLines.Count; i++)
                {
                    var array = SampleLines[i];
                    Array.Resize(ref array, ColumnMapping.Length);
                    SampleLines[i] = array;
                }
            }
            finally
            {
                IsBusy = false;
                RaisePropertyChanged(nameof(SampleLines));
            }
        }

        public void SetColumnMapping(int index, GBColumnName? val)
        {
            if (val == null)
            {
                ColumnMapping[index] = null;
                return;
            }

            for (var i = 0; i < ColumnMapping.Length; i++)
            {
                if (i != index && ColumnMapping[i] == val)
                {
                    ColumnMapping[i] = null;
                    RaisePropertyChanged(nameof(ColumnMapping));
                    break;
                }
            }
        }

        public async Task<bool> ImportAsync()
        {
            var columns = new GBColumnName?[] { GBColumnName.Amount, GBColumnName.Description, GBColumnName.Date };
            if (!columns.All(e => ColumnMapping.Any(c => c == e)))
            {
                errorService.AddWarning("Choose Date, Amount and Description");
                return false;
            }

            using var stream = ImportFile.OpenReadStream(MAX_STREAM_SIZE);
            try
            {
                IsBusy = true;
                await importApi.ImportAsync(AccountId.Value, stream, ColumnMapping);
            }
            finally
            {
                IsBusy = false;
            }

            errorService.AddSuccess($"{ImportFile.Name} imported");
            return true;
        }

        public List<string[]> SampleLines { get; set; }
        public GBColumnName?[] ColumnMapping { get; set; }

        public Guid? AccountId { get; set; }
        public List<Account> Accounts { get; set; }

        public IBrowserFile ImportFile { get; set; }

        public bool CanImport => ImportFile != null && AccountId != null;
    }
}
