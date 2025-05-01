using EventHub.ViewModels.Event;

namespace EventHub.Views.Event;

public partial class EventToUserPage : ContentPage
{
	public EventToUserPage(EventToUserViewModel eventToUserViewModel)
	{
		InitializeComponent();
        BindingContext = eventToUserViewModel;
    }
}