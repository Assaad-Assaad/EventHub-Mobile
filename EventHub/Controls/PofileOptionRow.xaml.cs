namespace EventHub.Controls;

public partial class PofileOptionRow : ContentView
{

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(PofileOptionRow), string.Empty);
    public PofileOptionRow()
	{
		InitializeComponent();
	}

	

    public string Text
    {
        get => GetValue(TextProperty)as string;
        set => SetValue(TextProperty, value);
    }

    public event EventHandler<string> Tapped;
    public void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        Tapped?.Invoke(this, Text);
    }
}