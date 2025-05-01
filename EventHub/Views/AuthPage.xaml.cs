using CommunityToolkit.Mvvm.Input;
using EventHub.Services;
using EventHub.ViewModels;

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


    protected override void OnAppearing()
    {
        base.OnAppearing();
        //_authViewModel.Initialize(_authViewModel.IsFirstTime);
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        //if (await _authService.IsAuthenticated())
        //{
        //    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        //}
           
           
        
        
    }



}