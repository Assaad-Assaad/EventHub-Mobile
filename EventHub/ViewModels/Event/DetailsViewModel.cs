using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHub.Models;
using EventHub.Services;
using EventHub.Views;
using System.Collections.Specialized;
using System.Diagnostics;

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
            if (SelectedEvent == null) return;

            if (_authService.CurrentUser == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please log in to favorite events", "OK");
                await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                return;
            }

            // Toggle favorite in service (local DB + will sync later)
            await _userEventService.ToggleFavoriteAsync(SelectedEvent.Id);

            // Refresh favorite status from DB instead of assuming it changed
            var userId = _authService.CurrentUser.Id;
            var userEvent = await _userEventService.GetUserEventAsync(userId, SelectedEvent.Id);

            IsFavorite = userEvent?.IsFavorite ?? false;

            if (IsFavorite)
            {
                await ShowToastAsync("Event added to favorites");
            }
            else
            {
                await ShowToastAsync("Event removed from favorites");
            }
        }

        [RelayCommand]
        public async Task SignForAsync()
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
        }

    }       

      

}

