using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHub.Models;
using EventHub.Services;
using EventHub.Shared.Dtos;
using EventHub.Views;
using System.ComponentModel.Design;
using System.Diagnostics;



namespace EventHub.ViewModels
{

    public partial class AuthViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private bool _hasAttemptedAutoLogin;

        [ObservableProperty]
        private bool _isRegisteringMode;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        public bool IsNewUser => !string.IsNullOrWhiteSpace(Name);

        public AuthViewModel(AuthService authService)
        {
            _authService = authService;
        }

        public async Task InitializeAsync()
        {
            
            if (!_hasAttemptedAutoLogin)
            {
                _hasAttemptedAutoLogin = true;
                var result = await TrySilentLogin();
                if (result.IsSuccess)
                {
                    await NavigateToHomeAsync();
                }
            }
        }

        [RelayCommand]
        private async Task ToggleMode()
        {
            IsRegisteringMode = !IsRegisteringMode;
            await ClearFormAsync();
        }

        [RelayCommand]
        private async Task Submit()
        {
            if (!ValidateForm())
            {
                await ShowToastAsync("Please fill all required fields");
                return;
            }

            IsBusy = true;

            try
            {
                if (IsRegisteringMode)
                {
                    await HandleRegistration();
                }
                else
                {
                    await HandleLogin();
                }
            }
            catch (Exception ex)
            {
                await HandleError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task HandleRegistration()
        {
            var dto = new RegisterDto
            {
                Name = Name,
                Email = Email,
                Password = Password
            };

            var result = await _authService.RegisterAsync(dto);

            if (!result.IsSuccess)
            {
                await ShowToastAsync($"Registration failed: {result.Error}");
                return;
            }

            await ShowToastAsync("Registration successful! Please login");
            await ClearFormAsync();
            IsRegisteringMode = false;
        }

        private async Task HandleLogin()
        {
            var loginDto = new LoginDto { Email = Email, Password = Password };
            var result = await _authService.LoginAsync(loginDto);

            if (result.IsSuccess)
            {
                await NavigateToHomeAsync();
            }
            else
            {
                await ShowToastAsync($"Login failed: {result.Error}");
            }
        }

        private async Task<ApiResult<AuthDto>> TrySilentLogin()
        {
            try
            {
                // Attempt cached login without UI interaction
                var result = await _authService.TryOfflineLogin();
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Silent login failed: {ex.Message}");
                return ApiResult<AuthDto>.Fail("Auto-login failed");
            }
        }

        private bool ValidateForm()
        {
            var isValid = !string.IsNullOrWhiteSpace(Email) &&
                         !string.IsNullOrWhiteSpace(Password) &&
                         (IsRegisteringMode ? !string.IsNullOrWhiteSpace(Name) : true);

            if (IsRegisteringMode && !IsValidEmail(Email))
            {
                return false;
            }

            return isValid;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [RelayCommand]
        private async Task ClearFormAsync()
        {
            Name = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }

        [RelayCommand]
        private async Task NavigateToHomeAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }

        private async Task HandleError(Exception ex)
        {
            Debug.WriteLine($"Authentication error: {ex}");
            await ShowToastAsync("An error occurred. Please try again.");
        }

        [RelayCommand]
        private async Task SkipForNow() => await  NavigateToHomeAsync();


    }

}
