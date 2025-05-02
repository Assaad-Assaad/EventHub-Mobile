using EventHub.Data;
using EventHub.Models;
using EventHub.Utils;
using EventHub.Shared.Dtos;
using System.Diagnostics;
using System.Text.Json;

namespace EventHub.Services
{
    public class FavoritesService
    {
        private readonly DatabaseContext _context;
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public FavoritesService(DatabaseContext context, HttpClient httpClient, AuthService authService)
        {
            _context = context;
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<bool> ToggleFavoriteAsync(int eventId)
        {
            try
            {
                var userId = _authService.CurrentUser.Id;
                if (userId == 0)
                    return false;

                // Get existing UserEvent or create new one
                var userEvents = await _context.GetItemsAsync<UserEvent>(ue => 
                    ue.UserId == userId && ue.EventId == eventId);
                var existingUserEvent = userEvents.FirstOrDefault();

                if (existingUserEvent != null)
                {
                    // Toggle the favorite status
                    existingUserEvent.IsFavorite = !existingUserEvent.IsFavorite;
                    existingUserEvent.IsSynced = false; // Mark for sync
                    await _context.SaveItemAsync(existingUserEvent);
                }
                else
                {
                    // Create new UserEvent with favorite status
                    var userEvent = new UserEvent
                    {
                        UserId = userId,
                        EventId = eventId,
                        IsFavorite = true,
                        IsSignedIn = false,
                        IsSynced = false // Mark for sync
                    };
                    await _context.SaveItemAsync(userEvent);
                }

                // Clear cache to force refresh
                _cachedFavorites = null;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error toggling favorite: {ex.Message}");
                return false;
            }
        }

        private List<Models.Event> _cachedFavorites;

        public async Task<List<Models.Event>> GetFavoriteEventsAsync()
        {
            try
            {
                var userId = _authService.CurrentUser?.Id;
                if (userId == 0)
                    return new List<Models.Event>();

                // Get local favorites
                var localFavorites = await _context.GetItemsAsync<UserEvent>(ue => 
                    ue.UserId == userId && ue.IsFavorite);
                var eventIds = localFavorites.Select(f => f.EventId).ToList();
                return await _context.GetItemsAsync<Models.Event>(e => eventIds.Contains(e.Id));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting favorite events: {ex.Message}");
                return new List<Models.Event>();
            }
        }

        public async Task SyncFavoritesAsync()
        {
            if (!IsOnline())
                return;

            try
            {
                var userId = _authService.CurrentUser?.Id;
                if (userId == 0)
                    return;

                // Get all unsynced UserEvents
                var unsyncedFavorites = await _context.GetItemsAsync<UserEvent>(ue => 
                    ue.UserId == userId && !ue.IsSynced);

                foreach (var userEvent in unsyncedFavorites)
                {
                    try
                    {
                        var response = await _httpClient.PutAsync(
                            $"{AppConstants.BaseUrl}/api/user-events/toggle-favorite/{userEvent.EventId}", null);

                        if (response.IsSuccessStatusCode)
                        {
                            // Mark as synced
                            userEvent.IsSynced = true;
                            await _context.SaveItemAsync(userEvent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error syncing favorite {userEvent.EventId}: {ex.Message}");
                        // Continue with next item even if one fails
                    }
                }

                // Clear cache to force refresh
                _cachedFavorites = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during sync: {ex.Message}");
            }
        }

        public async Task GetEventDetailsAsync(int eventId)
        {
            try
            {
                var userId = _authService.CurrentUser?.Id;
                if (userId == 0)
                    return;
                if (!IsOnline())
                {
                    // Get local event details
                    var localEvent = await _context.GetItemByIdAsync<Models.Event>(eventId);
                    if (localEvent != null)
                    {
                        // Do something with the local event details
                    }
                }
                else
                {
                    var response = await _httpClient.GetAsync(
                        $"{AppConstants.BaseUrl}/api/user-events/{eventId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<ApiResult<EventDto>>(content);
                        if (result?.IsSuccess == true)
                        {
                            // Do something with the event details
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting event details: {ex.Message}");
            }
        }

        private bool IsOnline() => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }
} 