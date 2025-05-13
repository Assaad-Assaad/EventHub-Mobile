

namespace EventHub.Services
{
    public class CommonService
    {
        public string? Token { get; private set; }

        public event EventHandler LoginStatusChanged;

        public void ToggleLoginStatus() => LoginStatusChanged?.Invoke(this, EventArgs.Empty);

    }
}
