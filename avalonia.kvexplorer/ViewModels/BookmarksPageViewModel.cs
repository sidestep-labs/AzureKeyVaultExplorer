using kvexplorer.shared;
using kvexplorer.shared.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace avalonia.kvexplorer.ViewModels;

public partial class BookmarksPageViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly KvExplorerDbContext kvDbContext;
    private readonly KvExplorerDb db;


    public BookmarksPageViewModel()
    {
        _authService = new AuthService();
        kvDbContext = Defaults.Locator.GetRequiredService<KvExplorerDbContext>();
        db = Defaults.Locator.GetRequiredService<KvExplorerDb>();
        var xv =  db.GetQuickAccessItems();
        var x = kvDbContext.BookmarkedItems.Count();
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