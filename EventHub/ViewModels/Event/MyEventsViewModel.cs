


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
                    await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                    return;
                }

                // Sync favorites from the API to the local database if online
                if (IsOnline())
                {
                    Debug.WriteLine("Syncing events from API to local DB...");
                    await _syncCoordinator.SyncFavoritesAsync();
                    Debug.WriteLine("Sync completed.");
                }

                
                var userEvents = await _userEventService.GetFavoriteEvents();

                
                var localEvents = await _eventsService.GetAllEventsAsync();

                
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
            }, requireOnline: false);
        }



        // Sign up for an event (removes it from favorites)
        [RelayCommand]
        public async Task SignForEventAsync(Models.Event @event)
        {
            await RunBusyActionAsync(async () =>
            {
                if (_authService.CurrentUser == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Please log in to sign up for events", "OK");
                    await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                    return;
                }

                var confirm = await Shell.Current.DisplayAlert(
                    "Sign Up",
                    $"Are you sure you want to sign up for '{@event.Title}'?",
                    "Yes", "No");

                if (confirm)
                {
                    await Shell.Current.DisplayAlert("Thank you", "You have successfully signed up for this event and we will see you there!", "OK");

                    await _userEventService.RemoveEventFromFavorites(@event.Id);

                    
                    var eventToRemove = FavoriteEvents.FirstOrDefault(e => e.Id == @event.Id);
                    if (eventToRemove != null)
                        FavoriteEvents.Remove(eventToRemove);

                    NoEventsAvailable = !FavoriteEvents.Any();
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
                        await Shell.Current.DisplayAlert("Error", "Please log in to delete events", "OK");
                        await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                        return;
                    }
                    var confirm = await Shell.Current.DisplayAlert(
                        "Delete Event",
                        $"Are you sure you want to delete '{@event.Title}'?",
                        "Yes", "No");
                    if (confirm)
                    {
                        await _userEventService.RemoveEventFromFavorites(@event.Id);
                        
                        var eventToRemove = FavoriteEvents.FirstOrDefault(e => e.Id == @event.Id);
                        if (eventToRemove != null)
                            FavoriteEvents.Remove(eventToRemove);
                        NoEventsAvailable = !FavoriteEvents.Any();
                    }
                }, requireOnline: false);

        }



    }
} 