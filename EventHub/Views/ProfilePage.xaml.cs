using EventHub.ViewModels;

namespace EventHub.Views;

public partial class ProfilePage : ContentPage
{
	
	
	public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(ProfilePage), default(string));


    public ProfilePage(ProfileViewModel profileViewModel)
	{
		InitializeComponent();
        BindingContext = profileViewModel;
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }


    public event EventHandler<string> Tapped;

    public void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        
        
        Tapped?.Invoke(this, Text);
    }

    
}