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
        get => (string)(GetValue(TextProperty) ?? string.Empty);
        set => SetValue(TextProperty, value);
    }
}
