using EventHub.Api.Data;
using EventHub.Api.Entities;
using EventHub.Shared.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Api.Services
{
    public class EventsService
    {
        private readonly DataContext _context;

        public EventsService(DataContext context)
        {
            _context = context;

        }




        public async Task<ApiResult<EventDto[]>> GetAllEventsAsync()
        {
            var events = await _context.Events
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Image = e.Image,
                    Description = e.Description,
                    Date = e.Date,
                    Category = e.Category,
                    Location = e.Location
                })
                .ToArrayAsync();
            return ApiResult<EventDto[]>.Success(events);
        }


        public async Task<ApiResult<EventDto[]>> GetNewlyAddedEventsAsync(int count)
        {
            var events = await _context.Events
                  .OrderByDescending(e => e.Id)
                  .Take(count)
                  .Select(e => new EventDto
                  {
                      Id = e.Id,
                      Title = e.Title,
                      Image = e.Image,
                      Description = e.Description,
                      Date = e.Date,
                      Category = e.Category,
                      Location = e.Location
                  })
                  .ToArrayAsync();
            return ApiResult<EventDto[]>.Success(events);
        }

        public async Task<ApiResult<EventDto[]>> GetFilteredEventsAsync(
            string? title = null,
            string? category = null,
            DateTime? startDate = null,
            string? sortOrder = null)
        {
            var query = _context.Events.AsQueryable();

            
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(e => e.Title.ToLower().Contains(title.ToLower()));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category == category);
            }

            if (startDate.HasValue)
            {
                query = query.Where(e => e.Date >= startDate.Value);
            }

            
            switch (sortOrder?.ToLower())
            {
                case "nearest":
                    query = query.OrderBy(e => e.Date);
                    break;
                case "farthest":
                    query = query.OrderByDescending(e => e.Date);
                    break;
                default:
                    query = query.OrderBy(e => e.Date); 
                    break;
            }

            
            var events = await query
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Image = e.Image,
                    Description = e.Description,
                    Date = e.Date,
                    Category = e.Category,
                    Location = e.Location
                })
                .ToArrayAsync();
            return ApiResult<EventDto[]>.Success(events);
        }


        public async Task<ApiResult<EventDto>> GetEventByIdAsync(int id)
        {
            try
            {
                var eventDetails = await _context.Events
                    .Where(e => e.Id == id)
                    .Select(e => new EventDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Image = e.Image,
                        Description = e.Description,
                        Date = e.Date,
                        Category = e.Category,
                        Location = e.Location
                    })
                    .FirstOrDefaultAsync();

                if (eventDetails == null)
                {
                    return ApiResult<EventDto>.Fail("Event not found");
                }

                return ApiResult<EventDto>.Success(eventDetails);
            }
            catch (Exception ex)
            {
                return ApiResult<EventDto>.Fail($"An error occurred: {ex.Message}");
            }
        }


        public async Task<ApiResult> CreateEventAsync(EventDto eventDto)
        {
            try
            {
                var newEvent = new Event
                {
                    Title = eventDto.Title,
                    Image = eventDto.Image,
                    Description = eventDto.Description,
                    Date = eventDto.Date,
                    Category = eventDto.Category,
                    Location = eventDto.Location
                };
                await _context.Events.AddAsync(newEvent);
                await _context.SaveChangesAsync();
                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail($"An error occurred: {ex.Message}");
            }


        }


        public async Task<ApiResult> UpdateEventAsync(int id, EventDto eventDto)
        {
            try
            {
                var existingEvent = await _context.Events.FindAsync(id);
                if (existingEvent == null)
                {
                    return ApiResult.Fail("Event not found");
                }
                existingEvent.Title = eventDto.Title;
                existingEvent.Image = eventDto.Image;
                existingEvent.Description = eventDto.Description;
                existingEvent.Date = eventDto.Date;
                existingEvent.Category = eventDto.Category;
                existingEvent.Location = eventDto.Location;
                _context.Events.Update(existingEvent);
                await _context.SaveChangesAsync();
                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail($"An error occurred: {ex.Message}");
            }

        }


        public async Task<ApiResult> DeleteEventAsync(int id)
        {
            try
            {
                var existingEvent = await _context.Events.FindAsync(id);
                if (existingEvent == null)
                {
                    return ApiResult.Fail("Event not found");
                }
                _context.Events.Remove(existingEvent);
                await _context.SaveChangesAsync();
                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail($"An error occurred: {ex.Message}");
            }


        }

    }
}


