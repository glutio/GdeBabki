using GdeBabki.Client.Services;
using GdeBabki.Shared;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class ImportViewModel: ViewModelBase
    {
        public const int MAX_STREAM_SIZE = 1024 * 1024 * 2; /* 2 mb */
        private readonly AccountsApi accountsApi;
        private readonly ImportApi importApi;

        public ImportViewModel(AccountsApi accountsApi, ImportApi importApi)
        {
            this.accountsApi = accountsApi;
            this.importApi = importApi;
        }

        public override async Task OnInitializeAsync()
        {
            Accounts = await accountsApi.GetAccountsAsync();
            IsLoaded = true;
        }

        public async Task LoadSampleLinesAsync(int count)
        {
            SampleLines = null;
            ColumnBindings = null;

            using var stream = ImportFile.OpenReadStream(MAX_STREAM_SIZE);

            try
            {
                var parser = new CsvParser();
                SampleLines = await parser.LoadAsync(stream, count);

                if (SampleLines.Count > 0)
                {
                    ColumnBindings = new GBColumnName?[SampleLines[0].Length];
                }
            }
            finally
            {
                RaisePropertyChanged(nameof(SampleLines));
            }
        }

        public void SetColumnBinding(int index, GBColumnName? val)
        {
            if (val == null)
            {
                ColumnBindings[index] = null;
                return;
            }

            for (var i = 0; i < ColumnBindings.Length; i++)
            {
                if (i != index && ColumnBindings[i] == val)
                {
                    ColumnBindings[i] = null;
                    RaisePropertyChanged(nameof(ColumnBindings));
                    break;
                }
            }
        }

        public async Task ImportAsync()
        {
            using var stream = ImportFile.OpenReadStream(MAX_STREAM_SIZE);
            await importApi.ImportAsync(AccountId, stream, ColumnBindings);
        }

        public List<string[]> SampleLines { get; set; }
        public GBColumnName?[] ColumnBindings { get; set; }

        public Guid AccountId { get; set; }
        public List<Account> Accounts { get; set; }

        public IBrowserFile ImportFile { get; set; }

    }
}
