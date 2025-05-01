using EventHub.Data;
using EventHub.Models;
using EventHub.Shared.Dtos;
using EventHub.Utils;

using System.Diagnostics;
using System.Net.Http.Json;


namespace EventHub.Services
{
    public class EventsService
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseContext _context;
        private bool IsOnline() => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;


        public EventsService(HttpClient httpClient, DatabaseContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }


        public async Task<List<Event>> GetAllEventsAsync()
        {
            try
            {
                var events = await _context.GetAllItemsAsync<Event>();
                Debug.WriteLine($"Retrieved {events.Count} events.");
                return events;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllEventsAsync Error: {ex.Message}");
                return new List<Event>();
            }
        }

        public async Task<List<Event>> GetRecentEventsAsync(int count)
        {
            try
            {
                var all = await _context.GetAllItemsAsync<Event>();
                return all.OrderByDescending(e => e.Date).Take(count).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetRecentEventsAsync Error: {ex.Message}");
                return new List<Event>();
            }
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            try
            {
                return await _context.GetItemByIdAsync<Event>(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetEventByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Event>> GetFilteredEventsAsync(string? title, string? category, string? dateFilter)
        {
            try
            {
                var all = await _context.GetAllItemsAsync<Event>();

                
                if (!string.IsNullOrWhiteSpace(title))
                    all = all.Where(e => e.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

                
                if (!string.IsNullOrWhiteSpace(category) && category != "All")
                    all = all.Where(e => e.Category == category).ToList();

                
                all = dateFilter switch
                {
                    "Nearest → Farthest" => all.OrderBy(e => e.Date).ToList(),
                    "Farthest → Nearest" => all.OrderByDescending(e => e.Date).ToList(),
                    _ => all
                };

                return all;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetFilteredEventsAsync Error: {ex.Message}");
                return new List<Event>();
            }
        }

        public async Task<List<Event>> GetPaginatedEventsAsync(int skip, int take)
        {
            try
            {
                var all = await _context.GetAllItemsAsync<Event>();
                return all.Skip(skip).Take(take).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetPaginatedEventsAsync Error: {ex.Message}");
                return new List<Event>();
            }
        }








    }
}



