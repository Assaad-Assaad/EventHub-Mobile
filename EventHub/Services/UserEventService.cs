using EventHub.Data;
using EventHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Services
{
    public class UserEventService
    {
        private readonly DatabaseContext _context;
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly EventsService _eventsService;

        public UserEventService(DatabaseContext context, HttpClient httpClient, AuthService authService, EventsService eventsService)
        {
            _context = context;
            _httpClient = httpClient;
            _authService = authService;
            _eventsService = eventsService;
        }




        
    

        
        
        
        // Mark an event as favorite
        public async Task MarkEventAsFavorite(int eventId)
        {
            var userId = _authService.CurrentUser?.Id ?? 0;
            if (userId == 0)
            {
                throw new InvalidOperationException("No user is currently logged in");
            }

            var userEvent = await _context.GetItemAsync<UserEvent>(ue => ue.UserId == userId && ue.EventId == eventId);
            if (userEvent == null)
            {
                userEvent = new UserEvent
                {
                    UserId = userId,
                    EventId = eventId,
                    IsFavorite = true
                };
                await _context.SaveItemAsync(userEvent);

            }
            else
            {
                userEvent.IsFavorite = true;
                await _context.SaveItemAsync(userEvent);
            }
        }  

        // Remove an event from favorites
        public async Task RemoveEventFromFavorites(int eventId)     
        {
            var userId = _authService.CurrentUser?.Id ?? 0;
            if (userId == 0)
            {
                throw new InvalidOperationException("No user is currently logged in");
            }
            var userEvent = await _context.GetItemAsync<UserEvent>(ue => ue.UserId == userId && ue.EventId == eventId);
            if (userEvent == null)
            {
                throw new InvalidOperationException("Event not found");
            }
            await _context.DeleteItemAsync(userEvent);
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

      

      

      
    }
}
