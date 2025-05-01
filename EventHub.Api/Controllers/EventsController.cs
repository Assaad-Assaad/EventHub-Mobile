using EventHub.Api.Services;
using EventHub.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Api.Controllers
{
    [Route("api/events")]
    [ApiController]
    
    public class EventsController : ControllerBase
    {
        private readonly EventsService _eventsService;

        public EventsController(EventsService eventsService)
        {
            _eventsService = eventsService;
        }




        [HttpGet] // GET api/events
        public async Task<ApiResult<EventDto[]>> GetAllEvents()
        {
            return await _eventsService.GetAllEventsAsync();
        }




        [HttpGet("newly-added")] // GET api/events/newly-added
        public async Task<ApiResult<EventDto[]>> GetNewlyAddedEvents([FromQuery] int count = 6)
        {
            return await _eventsService.GetNewlyAddedEventsAsync(count);
        }

        
        [HttpGet("filterd")] // GET api/events
        public async Task<ApiResult<EventDto[]>> GetFilteredEvents(
            [FromQuery] string? title = null,
            [FromQuery] string? category = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] string? sortOrder = null)
        {
            return await _eventsService.GetFilteredEventsAsync(title, category, startDate, sortOrder);
        }

        
        [HttpGet("{id}")] // GET api/events/{id}
        public async Task<ApiResult<EventDto>> GetEventById(int id)
        {
            return await _eventsService.GetEventByIdAsync(id);
        }
        [HttpPost] // POST api/events
        public async Task<ApiResult> CreateEvent([FromBody] EventDto eventDto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResult.Fail("Invalid event data.");
            }
            return await _eventsService.CreateEventAsync(eventDto);
        }

        [HttpPut("{id}")] // PUT api/events/{id}
        public async Task<ApiResult> UpdateEvent(int id, [FromBody] EventDto eventDto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResult.Fail("Invalid event data.");
            }
            return await _eventsService.UpdateEventAsync(id, eventDto);
        }
        [HttpDelete("{id}")] // DELETE api/events/{id}
        public async Task<ApiResult> DeleteEvent(int id)
        {
            return await _eventsService.DeleteEventAsync(id);
        }
    }
}
