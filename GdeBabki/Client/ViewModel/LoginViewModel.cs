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
        private readonly UserService userService;

        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public bool IsNewUser { get; set; }

        public override void OnInitialized()
        {
            userService.LoginInfo = null;
            userService.IsLoggedIn = false;
            base.OnInitialized();
        }

        public LoginViewModel(UserApi userApi, ErrorService errorService, UserService userService)
        {
            this.userApi = userApi;
            this.errorService = errorService;
            this.userService = userService;
        }

        public async Task<bool> Login()
        {
            userService.LoginInfo = new LoginInfo() { UserName = UserName, Password = Password };
            try
            {
                await userApi.Login();
            }
            catch (Exception e)
            {
                userService.LoginInfo = null;
                errorService.AddError(e.ToString(), 10000000);
                return false;
            }

            errorService.AddInfo("Welcome to GdeBabki");
            userService.IsLoggedIn = true;
            return true;
        }

        public async Task<bool> Create()
        {
            userService.LoginInfo = new LoginInfo() { UserName = UserName, Password = Password };
            try
            {
                await userApi.Create();
            }
            catch (Exception e)
            {
                userService.LoginInfo = null;
                errorService.AddError(e.ToString());
                return false;
            }

            errorService.AddSuccess("New user created");
            errorService.AddInfo("Welcome to GdeBabki");
            userService.IsLoggedIn = true;
            return true;
        }
    }
}
