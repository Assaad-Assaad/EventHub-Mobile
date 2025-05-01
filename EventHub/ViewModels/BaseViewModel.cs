using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace EventHub.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;



        protected async Task RunBusyActionAsync(Func<Task> action, string errorMessage = "Something went wrong.")
        {
            if (!IsOnline())
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

        protected async Task<T> RunBusyFunctionAsync<T>(Func<Task<T>> action, string errorMessage = "Something went wrong.")
        {
            if (!IsOnline())
            {
                await ShowAlertAsync("No Internet", "An internet connection is required.", "OK");
                return default;
            }

            if (IsBusy)
                return default;

            try
            {
                IsBusy = true;
                return await action();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ {errorMessage}: {ex.Message}");
                await ShowAlertAsync("Error", errorMessage, "OK");
                return default;
            }
            finally
            {
                IsBusy = false;
            }
        }


        protected bool IsOnline() => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;


        protected async Task ShowToastAsync(string message) => await Toast.Make(message).Show();



        protected async Task ShowAlertAsync(string title, string message, string buttonText) =>
            await Shell.Current.DisplayAlert(title, message, buttonText);


        protected async Task<bool> ShowConfirmAsync(string title, string message, string okButtonText, string cancelButtonText) =>
            await Shell.Current.DisplayAlert(title, message, okButtonText, cancelButtonText);



    }

}
