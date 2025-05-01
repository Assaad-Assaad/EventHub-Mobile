using EventHub.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Services
{
    public class UserEventService
    {
        //private readonly DatabaseContext _context;
        //private readonly IUserEventsApi _userApi;

        //public UserEventService(DatabaseContext context, IUserEventsApi userApi)
        //{
        //    _context = context;
        //    _userApi = userApi;
        //}

        //// Mark an event as signed up by the user
        //public async Task MarkEventAsSignedInAsync(int userId, int eventId)
        //{
        //    var userEvent = await _context.GetUserEventAsync(userId, eventId);
        //    if (userEvent == null)
        //    {
        //        // Create new user-event mapping
        //        userEvent = new UserEvent { UserId = userId, EventId = eventId, IsSignedIn = true };
        //        await _context.AddItemAsync(userEvent);
        //    }
        //    else
        //    {
        //        userEvent.IsSignedIn = true;
        //        await _context.UpdateItemAsync(userEvent);
        //    }
        //}

        //// Mark an event as a favorite for the user
        //public async Task MarkEventAsFavoriteAsync(int userId, int eventId)
        //{
        //    var userEvent = await _context.GetUserEventAsync(userId, eventId);
        //    if (userEvent == null)
        //    {
        //        // Create new user-event mapping
        //        userEvent = new UserEvent { UserId = userId, EventId = eventId, IsFavorite = true };
        //        await _context.AddItemAsync(userEvent);
        //    }
        //    else
        //    {
        //        userEvent.IsFavorite = true;
        //        await _context.UpdateItemAsync(userEvent);
        //    }
        //}
    }

}
