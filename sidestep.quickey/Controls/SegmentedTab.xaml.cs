namespace sidestep.quickey.Controls;

public partial class SegmentedTab : ContentView
{
    public SegmentedTab()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty SelectedValueProperty = BindableProperty.Create(nameof(SelectedValue), typeof(string), typeof(SegmentedTab), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedValuePropertyChanged);

    public string SelectedValue
    {
        get => (string)GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    private static void OnSelectedValuePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var control = (SegmentedTab)bindable;
        //Do your logic here
    }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(string[]), typeof(SegmentedTab));

    public string[] ItemsSource
    {
        get => (string[])GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
}