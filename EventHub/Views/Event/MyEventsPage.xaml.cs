

namespace EventHub.Views.Event;

public partial class MyEventsPage : ContentPage
{
    private readonly MyEventsViewModel _myEventsViewModel;

    public MyEventsPage(MyEventsViewModel myEventsViewModel)
    {
        InitializeComponent();
        _myEventsViewModel = myEventsViewModel;
        BindingContext = _myEventsViewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
        await _myEventsViewModel.LoadFavoriteEventsAsync();
    }
}
