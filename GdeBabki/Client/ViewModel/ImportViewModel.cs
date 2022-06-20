using GdeBabki.Client.Services;
using GdeBabki.Shared;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
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

        protected override void OnDispose()
        {
            accountsApi.AccountsUpdated -= AccountsApi_AccountsUpdated;
            base.OnDispose();
        }

        private async void AccountsApi_AccountsUpdated(object sender, EventArgs e)
        {
            Accounts = await accountsApi.GetAccountsAsync();
            RaisePropertyChanged(nameof(Accounts));
        }

        public async Task LoadSampleLinesAsync(int count)
        {
            SampleLines = null;
            ColumnMapping = null;

            using var stream = ImportFile.OpenReadStream(MAX_STREAM_SIZE);

            try
            {
                var parser = new CsvParser();
                SampleLines = await parser.LoadAsync(stream, count);

                if (SampleLines.Count > 0)
                {
                    ColumnMapping = new GBColumnName?[SampleLines[0].Length];
                }
            }
            finally
            {
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
            await importApi.ImportAsync(AccountId.Value, stream, ColumnMapping);

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
