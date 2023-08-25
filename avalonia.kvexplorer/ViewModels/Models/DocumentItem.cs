using avalonia.kvexplorer.Views.CustomControls;
using FluentAvalonia.UI.Controls;

namespace avalonia.kvexplorer.ViewModels.Models;

public class DocumentItem
{
    public string Header { get; set; }

    public IconSource IconSource { get; set; }

    public string Content { get; set; }

    public Vault Vault { get; set; }
}
