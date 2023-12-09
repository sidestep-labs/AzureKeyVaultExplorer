using kvexplorer.shared;
using kvexplorer.shared.Database;
using System.Linq;

namespace kvexplorer.ViewModels;

public partial class BookmarksPageViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly KvExplorerDb db;


    public BookmarksPageViewModel()
    {
        _authService = new AuthService();
        db = Defaults.Locator.GetRequiredService<KvExplorerDb>();
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