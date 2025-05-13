

namespace EventHub.Services
{
    public class SyncCoordinator
    {

        private readonly HttpClient _httpClient;
        private readonly DatabaseContext _context;
        private readonly AuthService _authService;
        
       


        public SyncCoordinator(HttpClient httpClient, DatabaseContext context, AuthService authService)
        {
            _httpClient = httpClient;
            _context = context;
            _authService = authService;
        }




        public async Task SyncEventsAsync()
        {
            try
            {
                if (!IsOnline())
                {
                    Debug.WriteLine(" Offline - Skipping sync.");
                    return;
                }

                Debug.WriteLine(" Starting event sync...");


                var localEvents = await _context.GetAllItemsAsync<Event>();
                var localEventIds = localEvents.Select(e => e.Id).ToHashSet();


                var response = await _httpClient.GetAsync($"{AppConstants.BaseUrl}/api/events");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($" API request failed: {response.StatusCode}");
                    return;
                }

                var apiResult = await response.Content.ReadFromJsonAsync<ApiResult<EventDto[]>>();
                var serverEvents = apiResult?.Data ?? Array.Empty<EventDto>();
                var serverEventIds = serverEvents.Select(e => e.Id).ToHashSet();

                Debug.WriteLine($" Processing {serverEvents.Length} server events...");


                var idsToDelete = localEventIds.Except(serverEventIds);
                foreach (var id in idsToDelete)
                {
                    await _context.DeleteItemByIdAsync<Event>(id);
                    Debug.WriteLine($" Deleted local event {id}");
                }


                foreach (var serverEvent in serverEvents)
                {
                    var existing = localEvents.FirstOrDefault(e => e.Id == serverEvent.Id);

                    if (existing == null)
                    {
                        await _context.SaveItemAsync(new Event
                        {
                            Id = serverEvent.Id,
                            Title = serverEvent.Title,
                            Description = serverEvent.Description,
                            Image = serverEvent.Image,
                            Date = serverEvent.Date,
                            Category = serverEvent.Category,
                            Location = serverEvent.Location
                        });
                        Debug.WriteLine($"✅ Added new event {serverEvent.Id}");
                    }
                    else
                    {
                        existing.Title = serverEvent.Title;
                        existing.Description = serverEvent.Description;
                        existing.Image = serverEvent.Image;
                        existing.Date = serverEvent.Date;
                        existing.Category = serverEvent.Category;
                        existing.Location = serverEvent.Location;

                        await _context.SaveItemAsync(existing);
                        Debug.WriteLine($" Updated event {serverEvent.Id}");
                    }
                }


                await _context.UpdateLastSyncTime(DateTime.UtcNow);
                Debug.WriteLine("Sync completed successfully");

                // Notify UI
                MessagingCenter.Send(this, "EventsUpdated");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" Sync error: {ex.Message}");
            }
        }


       public async Task SyncUserDataAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return;

                var userId = new TokenService().GetUserIdFromToken(token);
                var response = await _httpClient.GetAsync($"/api/users/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<AuthDto>();
                    await _context.SaveItemAsync(new LoggedInUser
                    {
                        Id = userId,
                        Name = user.Name,
                        Email = user.Email,
                        TokenExpiration = DateTime.UtcNow.AddDays(7)
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"User sync error: {ex.Message}");
            }
        }


        public async Task SyncFavoritesAsync()
        {
            if (!IsOnline())
            {
                Debug.WriteLine("Offline - Skipping favorite sync.");
                return;
            }

            Debug.WriteLine("Starting favorite sync...");

            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("No auth token found.");
                return;
            }

            var userId = new TokenService().GetUserIdFromToken(token);
            if (userId <= 0)
            {
                Debug.WriteLine("Invalid user ID from token.");
                return;
            }

            try
            {
                // Get all local favorite records not yet synced
                var localUserEvent = await _context.GetAllItemsAsync<UserEvent>();
                var localFavorites = localUserEvent
                    .Where(ue => ue.UserId == userId && ue.IsFavorite && !ue.IsSynced)
                    .ToList();

                foreach (var favorite in localFavorites)
                {
                    var url = $"{AppConstants.BaseUrl}/api/user-events/set-favorite/{favorite.EventId}";

                    var content = new StringContent(favorite.IsFavorite.ToString(), Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Mark as synced in local DB
                        favorite.IsSynced = true;
                        await _context.SaveItemAsync(favorite);
                        Debug.WriteLine($" Synced favorite status for event {favorite.EventId}");
                    }
                    else
                    {
                        Debug.WriteLine($" Failed to sync favorite for event {favorite.EventId}: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error syncing favorites: {ex.Message}");
            }
        }

        private bool IsOnline()
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }
    }
}
