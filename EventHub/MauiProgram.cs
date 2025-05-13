

using Microsoft.Extensions.Logging;

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

           
            
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<TokenService>();
            builder.Services.AddSingleton<EventsService>();
            builder.Services.AddSingleton<CommonService>();
            builder.Services.AddSingleton<SyncCoordinator>();
            builder.Services.AddSingleton<DatabaseContext>();
            builder.Services.AddSingleton<UserEventService>();
            builder.Services.AddSingleton<HomePage>().AddSingleton<HomeViewModel>();
            builder.Services.AddTransient<AuthPage>().AddTransient<AuthViewModel>();
            builder.Services.AddTransient<ProfilePage>().AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<DetailsPage>().AddTransient<DetailsViewModel>();
            builder.Services.AddTransient<MyEventsPage>().AddTransient<MyEventsViewModel>();
            builder.Services.AddTransient<AllEventsPage>().AddTransient<AllEventsViewModel>();
            
            
            

            


            return builder.Build();
        }



    }
}

