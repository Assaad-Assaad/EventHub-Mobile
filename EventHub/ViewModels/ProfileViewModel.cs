using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHub.Data;
using EventHub.Services;
using EventHub.Shared.Dtos;
using EventHub.Utils;
using EventHub.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly DatabaseContext _context;
        private readonly HttpClient _httpClient;


        [ObservableProperty]
        private string _email;
        [ObservableProperty]
        private bool isLoggedIn;
        [ObservableProperty, NotifyPropertyChangedFor(nameof(Initials))]
        private string _name = "Not Logged In";

        public string Initials => string.IsNullOrEmpty(Name) ? "" : $"{Name[0]}{Name[1]}".ToUpper();

        public ProfileViewModel(AuthService authService, DatabaseContext context, HttpClient httpClient)
        {
            _authService = authService;
            _context = context;
            _httpClient = httpClient;

            LoadUserData();
        }


       



        [RelayCommand]
        private async Task LoginLogoutAsync()
        {
          if (!IsLoggedIn)
            {
              await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
            }
            else
            {
              await _authService.LogoutAsync();
                IsLoggedIn = false;
                Name = "Not Logged In";
                                                                                                                                                               

            }
        }


        private void LoadUserData()
        {
            var user = _authService.CurrentUser;
            if (user != null)
            {
                IsLoggedIn = true;
                Name = user.Name;
                Email = user.Email;
            }
            else
            {
                IsLoggedIn = false;
                Name = "Not Logged In";
                Email = string.Empty;
            }
        }




    }
}
