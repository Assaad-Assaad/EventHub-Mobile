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


        public async Task<ApiResult<EventDto[]>> GetUserEventsAsync(int userId)
        {
            var userEvents = await _context.UserEvents
                .Where(ue => ue.UserId == userId)
                .Select(ue => new EventDto
                {
                    Id = ue.Id,
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


        public async Task<ApiResult<EventDto[]>> GetUserFavoriteEventsAsync(int userId)
        {
            var userEvents = await _context.UserEvents
                .Where(ue => ue.UserId == userId && ue.IsFavorite)
                .Select(ue => new EventDto
                {
                    Id = ue.Id,
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
                        IsFavorite = false, // Default to not favorited
                        IsSignedIn = true   // Mark as signed in
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
                var userEvent = await _context.UserEvents
                    .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);

                if (userEvent != null)
                {
                    userEvent.IsFavorite = !userEvent.IsFavorite;
                }
                else
                {
                    userEvent = new UserEvent
                    {
                        UserId = userId,
                        EventId = eventId,
                        IsFavorite = true,
                        IsSignedIn = false
                    };
                    await _context.UserEvents.AddAsync(userEvent);
                }

                await _context.SaveChangesAsync();
                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail($"Failed to toggle favorite status: {ex.Message}");
            }
        }

    }
}
