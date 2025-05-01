using CommunityToolkit.Mvvm.ComponentModel;


namespace EventHub.ViewModels.Event
{
    [QueryProperty(nameof(SelectedEvent), "Event")]
    public partial class DetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private Models.Event _selectedEvent;
    }
}
