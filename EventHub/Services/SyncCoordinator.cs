using EventHub.Data;
using EventHub.Shared.Dtos;
using System.Diagnostics;
using System.Net.Http.Json;
using EventHub.Models;
using EventHub.Utils;
using System.Linq;

namespace EventHub.Services
{
    public class SyncCoordinator
    {

        private readonly HttpClient _httpClient;
        private readonly DatabaseContext _context;
        private readonly AuthService _authService;
        
        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);
        private bool _isSyncing = false;


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



       // sync user events from server to local database and vice versa
       public async Task SyncUserEventsAsync()
       {
        if (!IsOnline())
        {
            Debug.WriteLine("Offline - skipping user event sync");
            return;
        }

        try
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return;

            var userId = new TokenService().GetUserIdFromToken(token);
            if (userId == 0) return;

            Debug.WriteLine($"Starting user event sync for user {userId}...");

            // Get local user events
            var localUserEvents = await _context.GetItemsAsync<UserEvent>(ue => ue.UserId == userId);
            Debug.WriteLine($"Found {localUserEvents.Count} local user events");

            // Get server user events
            var response = await _httpClient.GetAsync($"{AppConstants.BaseUrl}/api/user-events/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"API request failed: {response.StatusCode}");
                return;
            }

            var apiResult = await response.Content.ReadFromJsonAsync<ApiResult<UserEventDto[]>>();
            var serverUserEvents = apiResult?.Data ?? Array.Empty<UserEventDto>();
            Debug.WriteLine($"Retrieved {serverUserEvents.Length} user events from API");

            // Delete local user events that don't exist on server
            foreach (var localEvent in localUserEvents)
            {
                if (!serverUserEvents.Any(se => 
                    se.UserId == localEvent.UserId && 
                    se.EventId == localEvent.EventId))
                {
                    await _context.DeleteItemAsync(localEvent);
                    Debug.WriteLine($"Deleted local user event for event {localEvent.EventId}");
                }
            }

            // Add or update user events from server
            foreach (var serverEvent in serverUserEvents)
            {
                var existing = localUserEvents.FirstOrDefault(le => 
                    le.UserId == serverEvent.UserId && 
                    le.EventId == serverEvent.EventId);

                if (existing == null)
                {
                    var newUserEvent = new UserEvent
                    {
                        UserId = serverEvent.UserId,
                        EventId = serverEvent.EventId,
                        IsFavorite = serverEvent.IsFavorite,
                        IsSignedIn = serverEvent.IsSignedIn,
                        IsSynced = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.SaveItemAsync(newUserEvent);
                    Debug.WriteLine($"Added new user event for event {serverEvent.EventId}");
                }
                else
                {
                    existing.IsFavorite = serverEvent.IsFavorite;
                    existing.IsSignedIn = serverEvent.IsSignedIn;
                    existing.IsSynced = true;
                    await _context.SaveItemAsync(existing);
                    Debug.WriteLine($"Updated user event for event {serverEvent.EventId}");
                }
            }

            await _context.UpdateLastSyncTime(DateTime.UtcNow);
            Debug.WriteLine("User event sync completed successfully");
            MessagingCenter.Send(this, "UserEventsUpdated");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"User event sync error: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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

        private bool IsOnline()
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }
    }
}
