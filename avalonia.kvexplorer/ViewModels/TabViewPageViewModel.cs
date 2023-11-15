using System;
using System.Collections.ObjectModel;
using avalonia.kvexplorer.ViewModels;
using avalonia.kvexplorer.ViewModels.Models;
using avalonia.kvexplorer.Views.Pages;
using Avalonia.Styling;
using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Data;
using kvexplorer.shared;
using kvexplorer.shared.Database;

namespace avalonia.kvexplorer.ViewModels;

public partial class TabViewPageViewModel : ViewModelBase
{

    public TabViewPageViewModel()
    {
        Documents = new ObservableCollection<TabViewItem>();
        for (int i = 0; i < 3; i++)
        {
            Documents.Add(AddDocument(i));
        }
    }
     
    [ObservableProperty]
    public ObservableCollection<TabViewItem> documents;

    public ObservableCollection<TabViewItem> KeyBindingDocuments { get; }

    [ObservableProperty]
    public TabViewItem selectedItem;

    //public DocumentItem KeyBindingSelectedDocument
    //{
    //    get => _keybindingSelectedDocument;
    //    set => RaiseAndSetIfChanged(ref _keybindingSelectedDocument, value);
    //}

    public string KeyBindingText { get; set; }

    [RelayCommand]
    private void AddDocument()
    {
        var doc = AddDocument(Documents.Count);
        Documents.Add(doc);
    }

    private TabViewItem AddDocument(int index)
    {
        var tab = new TabViewItem
        {
            Header = $"My document {index}",
        };

        switch (index % 3)
        {
            case 0:
                tab.IconSource = new SymbolIconSource { Symbol = Symbol.Document };
                tab.Content = new VaultPage();
                break;

            case 1:
                tab.IconSource = new SymbolIconSource { Symbol = Symbol.Star };
                tab.Content = new VaultPage();
                break;

            case 2:
                tab.IconSource = new SymbolIconSource { Symbol = Symbol.Open };
                tab.Content = new VaultPage();
                break;
        }

        return tab;
    }

    [RelayCommand]
    private void AddVaultPage(KeyVaultData model) 
    {
        var tab = new TabViewItem
        {
            Header = model.Name,
            IconSource = new SymbolIconSource { Symbol = Symbol.ProtectedDocument },
            Content = new VaultPage(model.Properties.VaultUri)
        };
        Documents.Add(tab);
        SelectedItem = tab;
    }

}