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
            await RunBusyActionAsync(async () =>
            {
                if (_authService.CurrentUser == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Please log in to view your events", "OK");
                    return;
                }
                if (IsOnline())
                {
                    Debug.WriteLine("Syncing events from API to local DB...");
                    await _syncCoordinator.SyncFavoritesAsync();
                    Debug.WriteLine("Sync completed.");
                }
                else
                {
                    Debug.WriteLine("No internet. Loading events from local DB...");
                }
                var userEvents = await _userEventService.GetFavoriteEvents();
                FavoriteEvents.Clear();
                foreach (var userEvent in userEvents)
                {
                    var evt = await _eventsService.GetEventByIdAsync(userEvent.EventId);
                    if (evt != null)
                    {
                        FavoriteEvents.Add(evt);
                    }
                }
                NoEventsAvailable = !FavoriteEvents.Any();
            }, requireOnline: false);
        }


        [RelayCommand]
        public async Task SignForEventAsync(Models.Event @event)
        {
            await RunBusyActionAsync(async () =>
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
                    await ShowAlertAsync("Thank You",
                        "You have successfully signed up for this event and we will see you there!", "OK");
                    await _userEventService.RemoveEventFromFavorites(@event.Id);
                    await LoadFavoriteEventsAsync();
                }
            }, requireOnline: false);
        }


        [RelayCommand]
        public async Task DeleteEventAsync(Models.Event @event)
        {
           await RunBusyActionAsync(async () =>
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
                    await LoadFavoriteEventsAsync(); // Or remove directly from collection
                }
            }, requireOnline: false);
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            await LoadFavoriteEventsAsync();
        }





        //[RelayCommand]
        //public async Task LoadFavoriteEventsAsync()
        //{
        //    if (_authService.CurrentUser == null)
        //    {
        //        await Shell.Current.DisplayAlert("Error", "Please log in to view your events", "OK");
        //        return;
        //    }

        //    if (IsBusy) return;
        //    IsBusy = true;

        //    try
        //    {

        //        var userEvents = await _userEventService.GetFavoriteEvents();


        //        if (IsOnline())
        //        {
        //            await _syncCoordinator.SyncFavoritesAsync();
        //        }

        //        // Load events from local DB, not API
        //        var localEvents = await _eventsService.GetAllEventsAsync(); // Should be local-only
        //        var eventsDict = localEvents.ToDictionary(e => e.Id);

        //        FavoriteEvents.Clear();

        //        foreach (var userEvent in userEvents)
        //        {
        //            if (eventsDict.TryGetValue(userEvent.EventId, out var evt))
        //            {
        //                FavoriteEvents.Add(evt);
        //            }
        //        }

        //        NoEvents = !FavoriteEvents.Any();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error loading events: {ex.Message}");
        //        await Shell.Current.DisplayAlert("Error", "Could not load your events.", "OK");
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}


        //[RelayCommand]
        //public async Task SignForEventAsync(Models.Event @event)
        //{
        //    if (_authService.CurrentUser == null)
        //    {
        //        await Shell.Current.DisplayAlert("Error", "Please log in to sign up for events", "OK");
        //        return;
        //    }

        //    var confirm = await Shell.Current.DisplayAlert(
        //        "Sign Up",
        //        $"Are you sure you want to sign up for '{@event.Title}'?",
        //        "Yes", "No");

        //    if (confirm)
        //    {

        //        await ShowAlertAsync("Thank You",
        //            "You have successfully signed up for this event and we will see you there!", "OK");


        //        await _userEventService.RemoveEventFromFavorites(@event.Id);


        //        await LoadFavoriteEventsAsync();
        //    }
        //}


        //[RelayCommand]
        //public async Task DeleteEventAsync(Models.Event @event)
        //{
        //    if (_authService.CurrentUser == null)
        //    {
        //        await Shell.Current.DisplayAlert("Error", "Please log in to delete events", "OK");
        //        return;
        //    }

        //    var result = await Shell.Current.DisplayAlert("Delete Event",
        //        "Are you sure you want to delete this event?", "Yes", "No");

        //    if (result)
        //    {
        //        await _userEventService.RemoveEventFromFavorites(@event.Id);
        //        await LoadFavoriteEventsAsync(); // Or remove directly from collection
        //    }
        //}




    }
} 