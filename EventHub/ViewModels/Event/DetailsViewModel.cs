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
            try
            {
                SelectedEvent = evt;
                await CheckFavoriteStatusAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing details view: {ex.Message}");
                await ShowAlertAsync("Error", "Failed to load event details", "OK");
            }
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
                var favorites = await _favoritesService.GetFavoriteEventsAsync();
                IsFavorite = favorites?.Any(e => e.Id == SelectedEvent?.Id) ?? false;
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

            if (SelectedEvent == null)
            {
                await ShowAlertAsync("Error", "Event information is missing", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                var success = await _favoritesService.ToggleFavoriteAsync(SelectedEvent.Id);
                
                if (success)
                {
                    // Update UI immediately since we're working offline-first
                    IsFavorite = !IsFavorite;
                    

                    // Try to sync in the background
                    _ = Task.Run(async () => await _favoritesService.SyncFavoritesAsync());
                }
                else
                {
                    await ShowAlertAsync("Error", "Failed to update favorite status", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error toggling favorite: {ex.Message}");
                await ShowAlertAsync("Error", "Failed to update favorite status", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

       

        async partial void OnSelectedEventChanged(Models.Event value)
        {
            if (value == null) return;

            IsBusy = true;

            try
            {
                // Check favorite status when event changes
                await CheckFavoriteStatusAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading event details: {ex.Message}");
                await ShowAlertAsync("Error", "Failed to load event details", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
