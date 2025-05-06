using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace EventHub.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;



        public BaseViewModel()
        {
            
        }

        protected async Task RunBusyActionAsync(Func<Task> action, string errorMessage = "Something went wrong.", bool requireOnline = false)
        {
            if (requireOnline && !IsOnline())
            {
                await ShowAlertAsync("No Internet", "An internet connection is required.", "OK");
                return;
            }

            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await action();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ {errorMessage}: {ex.Message}");
                await ShowAlertAsync("Error", errorMessage, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

    

        public bool IsOnline() => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        protected async Task ShowAlertAsync(string title, string message, string cancel)
        {
            await Shell.Current.DisplayAlert(title, message, cancel);
        }

        public async Task ShowToastAsync(string message)
        {
            await Toast.Make(message).Show();
        }

        

        public async Task<bool> ShowConfirmAsync(string title, string message, string okButtonText, string cancelButtonText) =>
            await Shell.Current.DisplayAlert(title, message, okButtonText, cancelButtonText);


        
    }
}
