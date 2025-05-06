using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHub.Data;
using EventHub.Models;
using EventHub.Services;
using EventHub.Views;
using EventHub.Views.Event;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;


namespace EventHub.ViewModels.Event
{
    public partial class MyEventsViewModel : BaseViewModel
    {
        
        private readonly SyncCoordinator _syncCoordinator;
        private readonly AuthService _authService;
        private readonly UserEventService _userEventService;
        private readonly EventsService _eventsService;

        [ObservableProperty]
        private bool _noEventsAvailable;

        private bool _isInitialized;
        [ObservableProperty]
        private Models.Event _selectedEvent;

        public ObservableCollection<Models.Event> FavoriteEvents { get; } = new();

        public MyEventsViewModel(SyncCoordinator syncCoordinator, AuthService authService, UserEventService userEventService, EventsService eventsService)
        {

            _authService = authService;
            _syncCoordinator = syncCoordinator;
            _userEventService = userEventService;
            _eventsService = eventsService;


        }


        [RelayCommand]
        public async Task LoadFavoriteEventsAsync()
        {
            if (_authService.CurrentUser == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please log in to view your events", "OK");
                return;
            }

            if (IsBusy) return;
            IsBusy = true;

            try
            {

                var userEvents = await _userEventService.GetFavoriteEvents();


                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    await _syncCoordinator.SyncFavoritesAsync();
                }

                // Load events from local DB, not API
                var localEvents = await _eventsService.GetAllEventsAsync(); // Should be local-only
                var eventsDict = localEvents.ToDictionary(e => e.Id);

                FavoriteEvents.Clear();

                foreach (var userEvent in userEvents)
                {
                    if (eventsDict.TryGetValue(userEvent.EventId, out var evt))
                    {
                        FavoriteEvents.Add(evt);
                    }
                }

                NoEventsAvailable = !FavoriteEvents.Any();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading events: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Could not load your events.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
        public async Task SignForEventAsync(Models.Event @event)
        {
           
                try
                {
                    if (_authService.CurrentUser == null)
                    {
                        await ShowAlertAsync("Error", "Please log in to sign up for events", "OK");
                        return;
                    }

                    var confirm = await Shell.Current.DisplayAlert(
                        "Sign Up",
                        $"Are you sure you want to sign up for '{@event.Title}'?",
                        "Yes", "No");

                    if (confirm)
                    {
                        await _userEventService.RemoveEventFromFavorites(@event.Id);
                        await ShowAlertAsync("Thank You",
                            "You have successfully signed up for this event and we will see you there!", "OK");
                        
                        
                        await LoadFavoriteEventsAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error signing up for event: {ex.Message}");
                    await Shell.Current.DisplayAlert("Error", "Could not sign up for the event.", "OK");
                }
            
        }


        [RelayCommand]
        public async Task DeleteEventAsync(Models.Event @event)
        {
           
                try
                {
                    if (_authService.CurrentUser == null)
                    {
                        await ShowAlertAsync("Error", "Please log in to delete events", "OK");
                        return;
                    }

                    var result = await Shell.Current.DisplayAlert("Delete Event",
                        "Are you sure you want to delete this event?", "Yes", "No");

                    if (result)
                    {
                        await _userEventService.RemoveEventFromFavorites(@event.Id);
                        // Ensure UI updates immediately
                        await LoadFavoriteEventsAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting event: {ex.Message}");
                    await Shell.Current.DisplayAlert("Error", "Could not delete the event.", "OK");
                }
            
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            await LoadFavoriteEventsAsync();
        }








    }
} 