using EventHub.Api.Data;
using EventHub.Api.Entities;
using EventHub.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Api.Services
{
    public class UserEventService
    {

        private readonly DataContext _context;
        public UserEventService(DataContext context)
        {
            _context = context;
        }


        // For later if we want to separate the favorite events from the signed in events
        
        public async Task<ApiResult<EventDto[]>> GetUserEventsAsync(int userId)
        {
            var userEvents = await _context.UserEvents
                .Where(ue => ue.UserId == userId)
                .Select(ue => new EventDto
                {
                    Id = ue.Event.Id,
                    Title = ue.Event.Title,
                    Image = ue.Event.Image,
                    Description = ue.Event.Description,
                    Date = ue.Event.Date,
                    Category = ue.Event.Category,
                    Location = ue.Event.Location,
                    
                })
                .ToArrayAsync();
            return ApiResult<EventDto[]>.Success(userEvents);
        }


        // For now, we will just go with the favorite events
        public async Task<ApiResult<EventDto[]>> GetUserFavoriteEventsAsync(int userId)
        {
            var userEvents = await _context.UserEvents
                .Where(ue => ue.UserId == userId && ue.IsFavorite)
                .Select(ue => new EventDto
                {
                    Id = ue.Event.Id,
                    Title = ue.Event.Title,
                    Image = ue.Event.Image,
                    Description = ue.Event.Description,
                    Date = ue.Event.Date,
                    Category = ue.Event.Category,
                    Location = ue.Event.Location,
                   
                })
                .ToArrayAsync();
            return ApiResult<EventDto[]>.Success(userEvents);
        }

        public async Task<ApiResult> SignInForEventAsync(int userId, int eventId)
        {
            try
            {
                var userEvent = await _context.UserEvents
                .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);

                if (userEvent == null)
                {
                    
                    userEvent = new UserEvent
                    {
                        UserId = userId,
                        EventId = eventId,
                        IsFavorite = false, 
                        IsSignedIn = true   
                    };
                    await _context.UserEvents.AddAsync(userEvent);
                }
                else
                {
                    
                    userEvent.IsSignedIn = true;

                    
                    userEvent.IsFavorite = false;
                }

                await _context.SaveChangesAsync();
                return ApiResult.Success();

            }
            catch (Exception ex)
            
            {
                return ApiResult.Fail($"An error occurred: {ex.Message}"); 
            }
        }


        public async Task<ApiResult> DeleteUserEventAsync(int userId, int eventId)
        {
            var userEvent = await _context.UserEvents
                .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);

            if (userEvent == null)
            {
                return ApiResult.Fail("User-event relationship not found.");
            }

            _context.UserEvents.Remove(userEvent);
            await _context.SaveChangesAsync();

            return ApiResult.Success();
        }

        public async Task<ApiResult> ToggleFavoritesAsync(int userId, int eventId)
        {
            try
            {
                // Verify user exists
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return ApiResult.Fail("User not found");
                }

                // Verify event exists
                var eventExists = await _context.Events.FindAsync(eventId);
                if (eventExists == null)
                {
                    return ApiResult.Fail("Event not found");
                }

                var userEvent = await _context.UserEvents
                    .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);

                if (userEvent != null)
                {
                    // Toggle the favorite status
                    userEvent.IsFavorite = !userEvent.IsFavorite;
                    Console.WriteLine($"Toggling favorite status for user {userId} and event {eventId} to {userEvent.IsFavorite}");
                }
                else
                {
                    // Create a new favorite record
                    userEvent = new UserEvent
                    {
                        UserId = userId,
                        EventId = eventId,
                        IsFavorite = true,
                        IsSignedIn = false
                    };
                    await _context.UserEvents.AddAsync(userEvent);
                    Console.WriteLine($"Creating new favorite record for user {userId} and event {eventId}");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine($"Successfully saved favorite status for user {userId} and event {eventId}");
                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ToggleFavoritesAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return ApiResult.Fail($"Failed to toggle favorite status: {ex.Message}");
            }




            /*
             *  public async Task<ApiResult<List<Event>>> GetUserFavoriteEventsAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return ApiResult<List<Event>>.Fail("User not found");
                }

                var userEvents = await _context.Events
                    .Include(e => e.UserEvents)
                    .Where(e => e.UserEvents.Any(ue => ue.UserId == userId))
                    .ToListAsync();

                return ApiResult<List<Event>>.Success(userEvents);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Event>>.Fail($"An error occurred: {ex.Message}");
            }
        }
             * 
             * 
             * 
             * 
             */
        }

    }
}
