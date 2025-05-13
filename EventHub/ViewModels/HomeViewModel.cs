



namespace EventHub.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly EventsService _eventService;
        private readonly SyncCoordinator _syncCoordinator;  
        private readonly AuthService _authService;
        private readonly DatabaseContext _context;
        private readonly UserEventService _userEventService;

        [ObservableProperty]
        private string _welcomeMessage;

        [ObservableProperty]
        private bool _noEventsAvailable;

       

        [ObservableProperty]
        private string _name = "Stranger";

        public ObservableCollection<Models.Event> RecentEvents { get; } = new();

        public HomeViewModel(DatabaseContext context,
                            EventsService eventService, 
                            SyncCoordinator syncCoordinator,
                            AuthService authService,
                            UserEventService userEventService)
        {
            _eventService = eventService;
            _syncCoordinator = syncCoordinator;
            _authService = authService;
            _context = context;
            _userEventService = userEventService;

            
            _authService.UserLoggedIn += OnUserLoggedIn;
            _authService.UserLoggedOut += OnUserLoggedOut;

            MessagingCenter.Subscribe<SyncCoordinator>(
                this, "EventsUpdated", async (sender) => await LoadRecentEventsAsync());

            // Load initial state
            LoadUserData();
        }

        private void OnUserLoggedIn(object sender, LoggedInUser user)
        {
            Name = user.Name;
            WelcomeMessage = $"Hello: {Name}";
        }

        private void OnUserLoggedOut(object sender, EventArgs e)
        {
            Name = "Stranger";
            WelcomeMessage = "Welcome!";
        }

        private void LoadUserData()
        {
            try
            {
                var currentUser = _authService.CurrentUser;
                if (currentUser != null)
                {
                    Name = currentUser.Name;
                    WelcomeMessage = $"Hello: {Name}";
                }
                else
                {
                    Name = "Stranger";
                    WelcomeMessage = "Welcome!";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading user: {ex.Message}");
            }
        }

      

        [RelayCommand]
        public async Task LoadRecentEventsAsync()
        {
            await RunBusyActionAsync(async () =>
            {
                if (IsOnline())
                {
                    await _syncCoordinator.SyncEventsAsync();
                }

                var events = await _eventService.GetRecentEventsAsync(6);

                RecentEvents.Clear();
                foreach (var evt in events)
                {
                    RecentEvents.Add(evt);
                }

                NoEventsAvailable = RecentEvents.Count == 0;
            }, requireOnline: false);
        }

        [RelayCommand]
        public async Task GoToAllEventsAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(AllEventsPage)}");
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


    }
}
