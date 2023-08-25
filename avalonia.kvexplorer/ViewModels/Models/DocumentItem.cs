using avalonia.kvexplorer.Views.CustomControls;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using System.Collections.ObjectModel;

namespace avalonia.kvexplorer.ViewModels.Models;

public partial class DocumentItem : ObservableObject
{
    public string Header { get; set; }

    public IconSource IconSource { get; set; }

    public string Content { get; set; }

    public Vault Vault { get; set; }

    [ObservableProperty]
    public ObservableCollection<SecretProperties> secretList;

}
