






namespace EventHub.ViewModels.Event
{
    public partial class AllEventsViewModel : BaseViewModel
    {
        private readonly EventsService _eventService;
        private readonly SyncCoordinator _syncCoordinator;

        public ObservableCollection<Models.Event> AllEvents { get; } = new();

        [ObservableProperty]
        private bool _noEventsAvailable;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private string _selectedCategory;

        [ObservableProperty]
        private string _selectedDate;

        



        public AllEventsViewModel(EventsService eventsService, SyncCoordinator syncCoordinator)
        {
            _eventService = eventsService;
            _syncCoordinator = syncCoordinator;
           
        }

        [RelayCommand]
        public async Task LoadAllEventsAsync()
        {
            await RunBusyActionAsync(async () =>
            {
                if (IsOnline())
                {
                    Debug.WriteLine("Syncing events from API to local DB...");
                    await _syncCoordinator.SyncEventsAsync();
                    Debug.WriteLine("Sync completed.");
                }
                else
                {
                    Debug.WriteLine("No internet. Loading events from local DB...");
                }

                var filteredEvents = await _eventService.GetFilteredEventsAsync(SearchText, SelectedCategory, SelectedDate);
                Debug.WriteLine($"Retrieved {filteredEvents?.Count ?? 0} events.");

                AllEvents.Clear();

                if (filteredEvents?.Count > 0)
                {
                    foreach (var evt in filteredEvents)
                        AllEvents.Add(evt);
                }

                NoEventsAvailable = filteredEvents?.Count == 0;
            }, requireOnline: false);
        }



        [RelayCommand]
        public async Task GoToDetailsAsync(Models.Event selectedEvent)
        {
            if (selectedEvent == null)
                return;
            await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
            {
                ["Event"] = selectedEvent
            });
        }


        partial void OnSearchTextChanged(string value)
        {
            LoadAllEventsCommand.Execute(null);
        }

        partial void OnSelectedCategoryChanged(string value)
        {
            LoadAllEventsCommand.Execute(null);
        }

        partial void OnSelectedDateChanged(string value)
        {
            LoadAllEventsCommand.Execute(null);
        }


    
    }

}


