using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class LoginViewModel: ViewModelBase
    {
        private readonly UserApi userApi;
        private readonly ErrorService errorService;

        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public bool IsNewUser { get; set; }

        public override void OnInitialize()
        {
            UserApi.LoginInfo = null;
            base.OnInitialize();
        }

        public LoginViewModel(UserApi userApi, ErrorService errorService)
        {
            this.userApi = userApi;
            this.errorService = errorService;
        }

        public override Task OnInitializeAsync()
        {
            UserApi.LoginInfo = null;
            return base.OnInitializeAsync();
        }

        public async Task<bool> Login()
        {
            var loginInfo = new LoginInfo() { UserName = UserName, Password = Password };
            UserApi.LoginInfo = loginInfo;

            try
            {
                await userApi.Login();
                errorService.AddError("Welcome to GdeBabki");
            }
            catch (Exception e)
            {
                errorService.AddError(e.ToString());
                return false;
            }

            return true;
        }

        public async Task<bool> Create()
        {
            var loginInfo = new LoginInfo() { UserName = UserName, Password = Password };
            UserApi.LoginInfo = loginInfo;
            try
            {
                await userApi.Create();
                errorService.AddError("User created");
            }
            catch (Exception e)
            {
                errorService.AddError(e.ToString());
                return false;
            }

            return true;
        }
    }
}
