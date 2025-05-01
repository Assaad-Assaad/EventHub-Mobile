using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHub.Models;
using EventHub.Services;
using EventHub.Views;
using EventHub.Views.Event;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EventHub.ViewModels.Event
{
    public partial class MyEventsViewModel : BaseViewModel
    {
        private readonly FavoritesService _favoritesService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private bool _noEventsAvailable;

        public ObservableCollection<Models.Event> FavoriteEvents { get; } = new();

        public MyEventsViewModel(FavoritesService favoritesService, AuthService authService)
        {
            _favoritesService = favoritesService;
            _authService = authService;
        }

        [RelayCommand]
        public async Task LoadEventsAsync()
        {
            if (_authService.CurrentUser == null)
            {
                await ShowAlertAsync("Login Required", "Please log in to view your events.", "OK");
                await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                return;
            }

            await RunBusyActionAsync(async () =>
            {
                var events = await _favoritesService.GetFavoriteEventsAsync();
                
                FavoriteEvents.Clear();
                foreach (var evt in events)
                {
                    FavoriteEvents.Add(evt);
                }

                NoEventsAvailable = FavoriteEvents.Count == 0;
            }, requireOnline: false);
        }

        [RelayCommand]
        private async Task ToggleFavoriteAsync(Models.Event selectedEvent)
        {
            if (selectedEvent == null) return;

            await RunBusyActionAsync(async () =>
            {
                var isFavorite = await _favoritesService.ToggleFavoriteAsync(selectedEvent.Id);
                if (!isFavorite)
                {
                    await ShowToastAsync("You've signed up for this event!");
                    await LoadEventsAsync();
                }
            }, requireOnline: false);
        }

        [RelayCommand]
        private async Task GoToDetailsAsync(Models.Event selectedEvent)
        {
            if (selectedEvent == null) return;

            await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
            {
                ["Event"] = selectedEvent
            });
        }
    }
} 