using EventHub.Data;
using EventHub.Services;
using EventHub.Views;
using Microsoft.Extensions.DependencyInjection;

namespace EventHub
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        public App(DatabaseContext context, AuthService authService)
        {
            InitializeComponent();
            InitializeDatabase(context);
            
            _authService = authService;

            // Initialize authentication and navigation
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _authService.InitializeSessionAsync();
                var initialRoute = _authService.CurrentUser != null ? $"//{nameof(HomePage)}" : $"//{nameof(AuthPage)}";
                await Shell.Current.GoToAsync(initialRoute);
            });
        }
        
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        private async void InitializeDatabase(DatabaseContext context)
        {
            await context.InitializeDatabaseAsync();
        }

        
    }
}