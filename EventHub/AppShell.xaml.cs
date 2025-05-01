using EventHub.Views;
using EventHub.Views.Event;

namespace EventHub
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
            //Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        }
    }
}
