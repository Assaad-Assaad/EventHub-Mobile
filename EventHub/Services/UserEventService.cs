


namespace EventHub.Services
{
    public class UserEventService
    {
        private readonly DatabaseContext _context;
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly EventsService _eventsService;
        private readonly SyncCoordinator _syncCoordinator;

        public UserEventService(DatabaseContext context, HttpClient httpClient, AuthService authService, EventsService eventsService, SyncCoordinator syncCoordinator)
        {
            _context = context;
            _httpClient = httpClient;
            _authService = authService;
            _eventsService = eventsService;
            _syncCoordinator = syncCoordinator;
        }

        // Mark an event as favorite
        public async Task MarkEventAsFavorite(int eventId)
        {
            var userId = _authService.CurrentUser?.Id ?? 0;
            if (userId == 0) throw new InvalidOperationException("No user logged in");

            var userEvent = await _context.GetItemAsync<UserEvent>(ue => ue.UserId == userId && ue.EventId == eventId);

            if (userEvent == null)
            {
                userEvent = new UserEvent
                {
                    UserId = userId,
                    EventId = eventId,
                    IsFavorite = true,
                    IsSignedIn = false,
                    IsSynced = false
                };
            }
            else
            {
                // Only update if needed
                if (userEvent.IsFavorite) return;

                userEvent.IsFavorite = true;
                userEvent.IsSynced = false;
            }

            await _context.SaveItemAsync(userEvent);
        }

        // Remove an event from favorites
        public async Task RemoveEventFromFavorites(int eventId)
        {
            var userId = _authService.CurrentUser?.Id ?? 0;
            if (userId == 0) throw new InvalidOperationException("No user logged in");

            var userEvent = await _context.GetItemAsync<UserEvent>(ue => ue.UserId == userId && ue.EventId == eventId);

            if (userEvent == null)
            {
                userEvent = new UserEvent
                {
                    UserId = userId,
                    EventId = eventId,
                    IsFavorite = false,
                    IsSignedIn = false,
                    IsSynced = false
                };
            }
            else
            {
                if (!userEvent.IsFavorite) return;

                userEvent.IsFavorite = false;
                userEvent.IsSynced = false;
            }

            await _context.SaveItemAsync(userEvent);
        }


        // Get favorite events
        public async Task<List<UserEvent>> GetFavoriteEvents()
        {
            var userId = _authService.CurrentUser?.Id ?? 0;
            if (userId == 0)
            {
                throw new InvalidOperationException("No user is currently logged in");
            }
            return await _context.GetItemsAsync<UserEvent>(ue => 
                ue.UserId == userId && 
                ue.IsFavorite == true);
        }

        // Get favorite events with details
        public async Task<List<Event>> GetFavoriteEventsWithDetails()
        {
            var userId = _authService.CurrentUser?.Id ?? 0;
            if (userId == 0)
                throw new InvalidOperationException("No user is currently logged in");

            var favoriteLinks = await _context.GetItemsAsync<UserEvent>(
                ue => ue.UserId == userId && ue.IsFavorite);

            var eventIds = favoriteLinks.Select(f => f.EventId).ToList();
            return await _context.GetItemsAsync<Event>(e => eventIds.Contains(e.Id));
        }


        // Toggle favorite status
        public async Task<bool> ToggleFavoriteAsync(int eventId)
        {
            var userId = _authService.CurrentUser?.Id ?? 0;
            if (userId == 0) throw new InvalidOperationException("No user logged in");

            var userEvent = await _context.GetItemAsync<UserEvent>(ue =>
                ue.UserId == userId && ue.EventId == eventId);

            bool isFavorite;
            if (userEvent == null || !userEvent.IsFavorite)
            {
                await MarkEventAsFavorite(eventId);
                isFavorite = true;
            }
            else
            {
                await RemoveEventFromFavorites(eventId);
                isFavorite = false;
            }

            // Trigger sync if online
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await _syncCoordinator.SyncFavoritesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error syncing favorites after toggle: {ex.Message}");
                    // Don't throw, as the local change was successful
                }
            }

            return isFavorite;
        }


        public async Task<UserEvent> GetUserEventAsync(int userId, int eventId)
        {
            return await _context.GetItemAsync<UserEvent>(ue => ue.UserId == userId && ue.EventId == eventId);
        }
    }
}
