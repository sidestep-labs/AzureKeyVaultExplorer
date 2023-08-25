using System.Collections.ObjectModel;
using avalonia.kvexplorer.ViewModels;
using avalonia.kvexplorer.ViewModels.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;

namespace avalonia.kvexplorer.ViewModels;

public partial class TabViewPageViewModel : ViewModelBase
{
    public TabViewPageViewModel()
    {
        Documents = new ObservableCollection<DocumentItem>();
        for (int i = 0; i < 3; i++)
        {
            Documents.Add(AddDocument(i));
        }

    }

    [ObservableProperty]
    public ObservableCollection<DocumentItem> documents;

    public ObservableCollection<DocumentItem> KeyBindingDocuments { get; }

    //public DocumentItem KeyBindingSelectedDocument
    //{
    //    get => _keybindingSelectedDocument;
    //    set => RaiseAndSetIfChanged(ref _keybindingSelectedDocument, value);
    //}


    public string KeyBindingText { get; set; }


    [RelayCommand]
    private void AddDocument(object obj)
    {
        var doc = AddDocument(Documents.Count);
        Documents.Add(doc);
    }


    private DocumentItem AddDocument(int index)
    {
        var tab = new DocumentItem
        {
            Header = $"My document {index}"
        };

        switch (index % 3)
        {
            case 0:
                tab.IconSource = new SymbolIconSource { Symbol = Symbol.Document };
                tab.Content = "This is a sample document. Switch tabs to view more.";
                break;

            case 1:
                tab.IconSource = new SymbolIconSource { Symbol = Symbol.Star };
                tab.Content = "This is another sample document. Switch tabs to view more.";
                break;

            case 2:
                tab.IconSource = new SymbolIconSource { Symbol = Symbol.Open };
                tab.Content = "This is yet another sample document. Switch tabs to view more.";
                break;
        }

        return tab;
    }

}


