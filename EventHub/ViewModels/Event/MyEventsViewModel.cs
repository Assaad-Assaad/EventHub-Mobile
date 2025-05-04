using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHub.Data;
using EventHub.Models;
using EventHub.Services;
using EventHub.Views;
using EventHub.Views.Event;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EventHub.ViewModels.Event
{
    public partial class MyEventsViewModel : BaseViewModel
    {
        
        private readonly SyncCoordinator _syncCoordinator;
        private readonly AuthService _authService;
        private readonly UserEventService _userEventService;
        private readonly EventsService _eventsService;

        [ObservableProperty]
        private bool _noEvents;

        private bool _isInitialized;
        [ObservableProperty]
        private Models.Event _selectedEvent;

        public ObservableCollection<Models.Event> Events { get; } = new();

        public MyEventsViewModel(SyncCoordinator syncCoordinator, AuthService authService, UserEventService userEventService, EventsService eventsService)
        {

            _authService = authService;
            _syncCoordinator = syncCoordinator;
            _userEventService = userEventService;
            _eventsService = eventsService;
        }


        [RelayCommand]
        public async Task LoadEventsAsync()
        {
            if (_authService.CurrentUser == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please log in to view your events", "OK");
                await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                return;
            }

            await RunBusyActionAsync(async () =>
            {
                if (IsOnline())
                {
                    await _syncCoordinator.SyncUserEventsAsync();
                }

                var userEvents = await _userEventService.GetFavoriteEvents();
                var events = await _eventsService.GetAllEventsAsync(); 

                Events.Clear();
                foreach (var userEvent in userEvents)
                {
                    var evt = events.FirstOrDefault(e => e.Id == userEvent.EventId);
                    if (evt != null)
                    {
                        Events.Add(evt);
                    }
                }

                NoEvents = !Events.Any();
            }, requireOnline: false);
        }
        

        [RelayCommand]
        public async Task SignForEvent()
        {
            if (_authService.CurrentUser == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please log in to sign for events", "OK");
                await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                return;
            }

            await ShowAlertAsync("Thank you", "You have successfully signed up for this event and we will see you there!", "OK");
            await _userEventService.RemoveEventFromFavorites( SelectedEvent.Id);
            await LoadEventsAsync();
        }

          public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            await LoadEventsAsync();
        }

    }
} 