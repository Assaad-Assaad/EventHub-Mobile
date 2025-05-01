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

            Task.Run(async () =>
            {
                await authService.InitializeSessionAsync();

                if (authService.CurrentUser != null)
                {
                    // Optional: Navigate to Home page directly if already logged in
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
                else
                {
                    await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                }
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

        //private async void InitializeAppAsync()
        //{
        //    await _authService.InitializeSessionAsync();

        //    if (_authService.CurrentUser != null)
        //    {
        //        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        //    }
        //    else
        //    {
        //        await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
        //    }
        //}
    }
}