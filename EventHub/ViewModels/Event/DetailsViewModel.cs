using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHub.Models;
using EventHub.Services;
using EventHub.Views;
using System.Diagnostics;

namespace EventHub.ViewModels.Event
{
    [QueryProperty(nameof(SelectedEvent), "Event")]
    public partial class DetailsViewModel : BaseViewModel
    {
        private readonly FavoritesService _favoritesService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private Models.Event _selectedEvent;

        [ObservableProperty]
        private bool _isFavorite;

        public DetailsViewModel(FavoritesService favoritesService, AuthService authService)
        {
            _favoritesService = favoritesService;
            _authService = authService;
        }

        public async Task InitializeAsync(Models.Event evt)
        {
            SelectedEvent = evt;
            await CheckFavoriteStatusAsync();
        }

        private async Task CheckFavoriteStatusAsync()
        {
            if (_authService.CurrentUser == null)
            {
                IsFavorite = false;
                return;
            }

            try
            {
                var favorite = await _favoritesService.GetFavoriteEventsAsync();
                IsFavorite = favorite.Any(e => e.Id == SelectedEvent.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking favorite status: {ex.Message}");
                IsFavorite = false;
            }
        }

        [RelayCommand]
        private async Task ToggleFavoriteAsync()
        {
            if (_authService.CurrentUser == null)
            {
                await ShowAlertAsync("Login Required", "Please log in to favorite events.", "OK");
                await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                return;
            }

            await RunBusyActionAsync(async () =>
            {
                IsFavorite = await _favoritesService.ToggleFavoriteAsync(SelectedEvent.Id);
            }, requireOnline: false);
        }
    }
}
