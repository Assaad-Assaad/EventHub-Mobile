using EventHub.ViewModels;
using EventHub.Views.Event;
using System.Threading.Tasks;

namespace EventHub.Views;

public partial class ProfilePage : ContentPage
{
	
    private readonly ProfileViewModel _profileViewModel;
    public ProfilePage(ProfileViewModel profileViewModel)
	{
		InitializeComponent();
        _profileViewModel = profileViewModel;
        BindingContext = _profileViewModel;

    }

    private async Task ProfileOptionRow_Tapped(object sender, string optionText)
    {
        switch (optionText)
        {
            case "My FavoriteEvents":
               // Shell.Current.GoToAsync($"//{nameof(MyEventsPage)}");
               await _profileViewModel.ShowToastAsync("My FavoriteEvents Clicked");
                break;
            
        }
    }

}