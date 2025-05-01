using CommunityToolkit.Maui;

using EventHub.Data;
using EventHub.Services;
using EventHub.Utils;
using EventHub.ViewModels;
using EventHub.ViewModels.Event;
using EventHub.Views;
using EventHub.Views.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;

namespace EventHub
{
    public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DatabaseContext>();
            builder.Services.AddSingleton<SyncCoordinator>();
            builder.Services.AddSingleton<EventsService>();
            builder.Services.AddSingleton<HomePage>().AddSingleton<HomeViewModel>();
            builder.Services.AddTransient<DetailsPage>().AddTransient<DetailsViewModel>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<TokenService>();
            builder.Services.AddTransient<MyEventsPage>().AddTransient<MyEventsViewModel>();
            builder.Services.AddTransient<AllEventsPage>().AddTransient<AllEventsViewModel>();
            builder.Services.AddTransient<AuthPage>().AddTransient<AuthViewModel>();
            builder.Services.AddTransient<ProfilePage>().AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<EventToUserPage>().AddTransient<EventToUserViewModel>();

            builder.Services.AddSingleton<HttpClient>();
            

            return builder.Build();
        }

        

    }
}

