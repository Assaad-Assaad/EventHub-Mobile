using EventHub.ViewModels;

namespace EventHub.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _homeViewModel;
    public HomePage(HomeViewModel homeViewModel)
	{
		InitializeComponent();
        _homeViewModel = homeViewModel;
        BindingContext = _homeViewModel; 
    }


    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _homeViewModel.InitializeAsync();
    }


   
}