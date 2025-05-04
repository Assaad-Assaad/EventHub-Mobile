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

            if (SelectedEvent.IsFavorite)
            {
                await _userEventService.RemoveEventFromFavorites(SelectedEvent.Id);
                SelectedEvent.IsFavorite = false;
            }
            else
            {
                await _userEventService.MarkEventAsFavorite(SelectedEvent.Id);
                SelectedEvent.IsFavorite = true;
            }
        }

        [RelayCommand]
        public async Task SignUpAsync()
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

