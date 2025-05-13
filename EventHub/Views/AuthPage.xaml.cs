using CommunityToolkit.Mvvm.Input;
using EventHub.Services;


namespace EventHub.Views;

public partial class AuthPage : ContentPage
{
    private readonly AuthViewModel _authViewModel;
    private readonly AuthService _authService;
    public AuthPage(AuthViewModel authViewModel, AuthService authService)
    {
        InitializeComponent();
        _authViewModel = authViewModel;
        BindingContext = _authViewModel;
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _authViewModel.InitializeAsync();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
           
           
    }



}