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

                if (!IsOnline())
                {
                    // Store locally for sync later
                    var userEvent = new UserEventDto
                    {
                        UserId = userId,
                        EventId = eventId,
                        IsFavorite = true,
                        IsSignedIn = false
                    };
                    await _context.SaveItemAsync(userEvent);
                    return true;
                }

                var response = await _httpClient.PutAsync(
                    $"{AppConstants.BaseUrl}/api/user-events/toggle-favorite/{eventId}", null);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error toggling favorite: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Models.Event>> GetFavoriteEventsAsync()
        {
            try
            {
                var userId = _authService.CurrentUser?.Id;
                if (userId == 0)
                    return new List<Models.Event>();

                if (!IsOnline())
                {
                    // Get local favorites
                    var localFavorites = await _context.GetItemsAsync<UserEventDto>(ue => 
                        ue.UserId == userId && ue.IsFavorite);
                    var eventIds = localFavorites.Select(f => f.EventId).ToList();
                    return await _context.GetItemsAsync<Models.Event>(e => eventIds.Contains(e.Id));
                }

                var response = await _httpClient.GetAsync(
                    $"{AppConstants.BaseUrl}/api/user-events/favorites");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResult<EventDto[]>>(content);
                    if (result?.IsSuccess == true)
                    {
                        return result.Data.Select(e => new Models.Event
                        {
                            Id = e.Id,
                            Title = e.Title,
                            Description = e.Description,
                            Date = e.Date,
                            Category = e.Category,
                            Location = e.Location,
                            Image = e.Image
                        }).ToList();
                    }
                }

                return new List<Models.Event>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting favorite events: {ex.Message}");
                return new List<Models.Event>();
            }
        }

        private bool IsOnline() => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }
} 