

namespace EventHub.Views.Event;

public partial class AllEventsPage : ContentPage
{
    private readonly AllEventsViewModel _allEventsViewModel;
    public AllEventsPage(AllEventsViewModel allEventsViewModel)
	{
		InitializeComponent();
        _allEventsViewModel = allEventsViewModel;
        BindingContext = _allEventsViewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
       await _allEventsViewModel.LoadAllEventsAsync();
    }
}