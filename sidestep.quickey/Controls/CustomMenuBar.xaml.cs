namespace sidestep.quickey.Controls;

public partial class CustomMenuBar : ContentView
{
    public CustomMenuBar()
    {
        InitializeComponent();
    }

    //void btnBack_Clicked(System.Object sender, System.EventArgs e)
    //{
    //    Navigation.PopAsync();
    //}

    //void btnHome_Clicked(System.Object sender, System.EventArgs e)
    //{
    //    Navigation.PopToRootAsync();
    //}

    //public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(
    //nameof(TitleText),
    //typeof(string),
    //typeof(CustomNavigationBar),
    //defaultValue: string.Empty,
    //BindingMode.OneWay,
    //propertyChanged: titleBindablePropertyChanged);

    //private static void titleBindablePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    var control = (CustomNavigationBar)bindable;
    //    control.lblBarTitle.Text = newValue.ToString();
    //    //throw new NotImplementedException();
    //}

    //public string TitleText
    //{
    //    get { return base.GetValue(TitleTextProperty).ToString(); }
    //    set { base.SetValue(TitleTextProperty, value); }
    //}
}