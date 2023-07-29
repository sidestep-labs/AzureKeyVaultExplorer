using kvexplorer.shared;

namespace avalonia.kvexplorer.ViewModels;

public partial class BookmarksPageViewModel : ViewModelBase
{
    private readonly AuthService _authService;

    public BookmarksPageViewModel(AuthService authService)
    {
        _authService = authService;
    }

    public BookmarksPageViewModel()
    {
        _authService = new AuthService();
    }

    /// <summary>
    /// The Title of this page
    /// </summary>
    public string Title => "Welcome to our Wizard-Sample.";

    /// <summary>
    /// The content of this page
    /// </summary>
    public string Message => "Press \"Next\" to register yourself.";
}