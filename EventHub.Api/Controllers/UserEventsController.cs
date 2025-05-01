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
            var userId = UserId;
            var result = await _userEventService.ToggleFavoritesAsync(userId, eventId);

            if (!result.IsSuccess)
            {
                return ApiResult.Fail("Faild Adding to favorite");
            }

            return ApiResult.Success();
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
        [HttpGet("")] // GET api/user-events
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
