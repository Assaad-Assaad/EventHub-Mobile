using EventHub.Api.Services;
using EventHub.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Api.Controllers
{
    [Route("api/user-events")]
    [ApiController]
    public class UserEventsController : AppBaseController
    {
        private readonly UserEventService _userEventService;

        public UserEventsController(UserEventService userEventService)
        {
            _userEventService = userEventService;
        }

        // Toggle Favorite Status
        [HttpPut("toggle-favorite/{eventId}")] //PUT api/user-events/toggle-favorite/{eventId}
        public async Task<ApiResult> ToggleFavorite(int eventId)
        {
            try
            {
                var userId = UserId;
                Console.WriteLine($"Attempting to toggle favorite for event {eventId} by user {userId}");

                var result = await _userEventService.ToggleFavoritesAsync(userId, eventId);

                if (!result.IsSuccess)
                {
                    Console.WriteLine($"Failed to toggle favorite");
                    return ApiResult.Fail("");
                }

                Console.WriteLine($"Successfully toggled favorite for event {eventId} by user {userId}");
                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ToggleFavorite endpoint: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return ApiResult.Fail($"An error occurred while toggling favorite: {ex.Message}");
            }
        }

        // Sign In for an Event
        [HttpPut("sign-in/{eventId}")] //PUT api/user-events/sign-in/{eventId}
        public async Task<ApiResult> SignInForEvent(int eventId)
        {
            var userId = UserId; 
            var result = await _userEventService.SignInForEventAsync(userId, eventId);

            if (!result.IsSuccess)
            {
                return ApiResult.Fail("Faild to sign for this event");
            }

            return ApiResult.Success();
        }

        
        [HttpDelete("{eventId}")] // DELETE api/user-events/{eventId}
        public async Task<ApiResult> DeleteUserEvent(int eventId)
        {
            var userId = UserId;
            var result = await _userEventService.DeleteUserEventAsync(userId, eventId);

            if (!result.IsSuccess)
            {
                return ApiResult.Fail("Faild to delete this event");
            }

            return ApiResult.Success();
        }

        // Get Favorite Events
        [HttpGet("favorites")] // GET api/user-events/favorites
        public async Task<ApiResult<EventDto[]>> GetFavoriteEvents()
        {
            var userId = UserId;
            var result = await _userEventService.GetUserFavoriteEventsAsync(userId);
            if (!result.IsSuccess)
            {
                return ApiResult<EventDto[]>.Fail("Faild to get favorite events");
            }
            return result;
        }

        // Get User Events
        [HttpGet("{userId}")] // GET api/user-events
        public async Task<ApiResult<EventDto[]>> GetUserEvents()
        {
            var userId = UserId;
            var result = await _userEventService.GetUserEventsAsync(userId);
            if (!result.IsSuccess)
            {
                return ApiResult<EventDto[]>.Fail("Faild to get user events");
            }
            return result;
        }
    }
}
