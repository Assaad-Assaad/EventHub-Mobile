

namespace EventHub.ViewModels.Event
{
    [QueryProperty(nameof(SelectedEvent), "Event")]
    public partial class DetailsViewModel : BaseViewModel
    {
        private readonly EventsService _eventsService;
        private readonly UserEventService _userEventService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private int _eventId;

        [ObservableProperty]
        private Models.Event _selectedEvent;

        [ObservableProperty]
        private bool _isFavorite;

        public DetailsViewModel(EventsService eventsService, UserEventService userEventService, AuthService authService)
        {
            _eventsService = eventsService;
            _userEventService = userEventService;
            _authService = authService;
        }


        [RelayCommand]
        public async Task ToggleFavoriteAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (SelectedEvent == null) return;
                if (_authService.CurrentUser == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Please log in to favorite events", "OK");
                    await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                    return;
                }
                
                // Toggle favorite status and get the new state
                IsFavorite = await _userEventService.ToggleFavoriteAsync(SelectedEvent.Id);
                
                // Show appropriate message
                if (IsFavorite)
                {
                    await ShowToastAsync("Event added to favorites");
                }
                else
                {
                    await ShowToastAsync("Event removed from favorites");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error toggling favorite: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Could not update favorite status", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SignForAsync()
        {
            await RunBusyActionAsync(async () =>
            {
                if (SelectedEvent == null) return;
                if (_authService.CurrentUser == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Please log in to sign up for events", "OK");
                    await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                    return;
                }
                // here I just want to show a thank you message i do not want to make any changes to the database
                await ShowAlertAsync("Thank you", "You have successfully signed up for this event and we will see you there!", "OK");
            });
        }



    }       

      
}

