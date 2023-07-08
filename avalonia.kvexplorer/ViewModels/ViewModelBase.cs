using CommunityToolkit.Mvvm.ComponentModel;

namespace avalonia.kvexplorer.ViewModels;

public class ViewModelBase : ObservableObject
{
    public string NavHeader { get; set; }

    public string IconKey { get; set; }

    public bool ShowsInFooter { get; set; }

}
