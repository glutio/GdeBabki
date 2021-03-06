using GdeBabki.Client.Services;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class LoginViewModel: ViewModelBase
    {
        private readonly UserApi userApi;
        private readonly ErrorService errorService;
        private readonly UserService userService;
        private readonly IWebAssemblyHostEnvironment webAssemblyHostEnvironment;

        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public bool IsNewUser { get; set; }

        public override Task OnInitializedAsync()
        {
            userService.LoginInfo = null;
            userService.IsLoggedIn = false;
            IsLoaded = true;
            return base.OnInitializedAsync();
        }

        public LoginViewModel(UserApi userApi, ErrorService errorService, UserService userService, IWebAssemblyHostEnvironment webAssemblyHostEnvironment)
        {
            this.userApi = userApi;
            this.errorService = errorService;
            this.userService = userService;
            this.webAssemblyHostEnvironment = webAssemblyHostEnvironment;
        }

        public async Task<bool> Login()
        {
            if (!webAssemblyHostEnvironment.IsDevelopment() && (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password)))
            {
                errorService.AddWarning("Please enter username and password");
                return false;
            }
            
            userService.LoginInfo = new LoginInfo() { UserName = UserName, Password = Password };
            try
            {
                IsBusy = true;
                await userApi.Login();
                userService.IsLoggedIn = true;
            }
            catch (Exception e)
            {
                userService.LoginInfo = null;
                errorService.AddError(e.ToString());
                return false;
            }
            finally
            {
                IsBusy = false;
            }

            errorService.AddInfo("Welcome to GdeBabki");
            return true;
        }

        public async Task<bool> Create()
        {
            if (!webAssemblyHostEnvironment.IsDevelopment() && (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword)))
            {
                errorService.AddWarning("Please enter username, password and confirm password");
                return false;
            }

            if (Password != ConfirmPassword)
            {
                errorService.AddError("Passwords do not match");
                return false;
            }

            userService.LoginInfo = new LoginInfo() { UserName = UserName, Password = Password };
            try
            {
                IsBusy = true;
                await userApi.Create();
            }
            catch (Exception e)
            {
                userService.LoginInfo = null;
                errorService.AddError(e.ToString());
                return false;
            }
            finally
            {
                IsBusy = false;
            }

            errorService.AddSuccess("New user registered successfully");
            errorService.AddInfo("Welcome to GdeBabki");
            userService.IsLoggedIn = true;
            return true;
        }
    }
}
