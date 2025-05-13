

namespace EventHub.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly DatabaseContext _context;
        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private string _email;
        
        [ObservableProperty]
        private bool _isLoggedIn;
        
        [ObservableProperty, NotifyPropertyChangedFor(nameof(Initials))]
        private string _name = "Not Logged In";

        public string Initials => string.IsNullOrEmpty(Name) ? string.Empty : Name.Substring(0, 1).ToUpper();

        public ProfileViewModel(AuthService authService, DatabaseContext context, HttpClient httpClient)
        {
            _authService = authService;
            _context = context;
            _httpClient = httpClient;

            // Subscribe to authentication state changes
            _authService.UserLoggedIn += OnUserLoggedIn;
            _authService.UserLoggedOut += OnUserLoggedOut;

            // Load initial state
            LoadUserData();
        }

        private void OnUserLoggedIn(object sender, LoggedInUser user)
        {
            IsLoggedIn = true;
            Name = user.Name;
            Email = user.Email;
        }

        private void OnUserLoggedOut(object sender, EventArgs e)
        {
            IsLoggedIn = false;
            Name = "Not Logged In";
            Email = string.Empty;
        }

        [RelayCommand]
        private async Task LoginLogoutAsync()
        {
            if (!IsLoggedIn)
            {
                await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
            }
            else
            {
                var confirm = await ShowConfirmAsync(
                    "Logout",
                    "Are you sure you want to logout?",
                    "Yes",
                    "No");

                if (confirm)
                {
                    await _authService.LogoutAsync();
                    await ShowToastAsync("Logged out successfully");
                }
            }
        }

        public void LoadUserData()
        {
            var user = _authService.CurrentUser;
            if (user != null)
            {
                IsLoggedIn = true;
                Name = user.Name;
                Email = user.Email;
            }
            else
            {
                IsLoggedIn = false;
                Name = "Not Logged In";
                Email = string.Empty;
            }
        }

       


        [RelayCommand]
        public async Task GoToMyEventsAsync(string rowText)
        {
            switch (rowText)
            {
                case "My Events":
                    await Shell.Current.GoToAsync($"//{nameof(MyEventsPage)}");
                    break;
                case "Name":
                    // Handle name-related action
                    break;
                case "Email":
                    // Handle email-related action
                    break;
                    // Add more cases as needed
            }
        }


    }

}
