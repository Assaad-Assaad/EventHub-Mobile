

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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _profileViewModel.LoadUserData();
    }

    private void PofileOptionRow_Focused(object sender, FocusEventArgs e)
    {

    }
}