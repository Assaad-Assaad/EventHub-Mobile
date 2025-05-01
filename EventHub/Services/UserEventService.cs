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

        public UserEventService(DatabaseContext context, HttpClient httpClient, AuthService authService)
        {
            _context = context;
            _httpClient = httpClient;
            _authService = authService;
        }



        


    }

}
