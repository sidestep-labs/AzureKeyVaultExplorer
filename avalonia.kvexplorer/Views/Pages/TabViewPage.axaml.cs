using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Database;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace avalonia.kvexplorer.Views.Pages;

public partial class TabViewPage : UserControl
{

    public TabViewPage()
    {
        InitializeComponent();
        //var vm = new TabViewPageViewModel();

        DataContext = Defaults.Locator.GetRequiredService<TabViewPageViewModel>();
    }

    //private void TabView_AddButtonClick(TabView sender, EventArgs args)
    //{
    //    (sender.TabItems as IList).Add(CreateNewTab(sender.TabItems.Count()));
    //}

    private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        (sender.TabItems as IList).Remove(args.Tab);
    }

    //private DocumentItem CreateNewTab(int index)
    //{
    //    var tvi = new DocumentItem
    //    {
    //        Header = $"Vault Item {index}",
    //        Content = $"Vault Item {index}",
    //        IconSource = new SymbolIconSource { Symbol = Symbol.List },
    //        Vault = new Vault("tet")
    //    };
    //    return tvi;
    //}

    private void TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        (DataContext as TabViewPageViewModel).Documents.Remove(args.Item as TabViewItem);
    }

    /* FA demo app Logic for tab dragging and dropping */

    public static readonly string DataIdentifier = "MyTabItemFromMain";

    private void TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
    {
        // Set the data payload to the drag args
        args.Data.SetData(DataIdentifier, args.Tab);

        // Indicate we can move
        args.Data.RequestedOperation = DragDropEffects.Move;
    }

    private void TabStripDrop(object sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataIdentifier) && e.Data.Get(DataIdentifier) is TabViewItem tvi)
        {
            var destinationTabView = sender as TabView;
            // While the TabView's internal ListView handles placing an insertion point gap, it
            // doesn't actually hold that position upon drop - meaning you now must calculate
            // the approximate position of where to insert the tab
            int index = -1;

            for (int i = 0; i < destinationTabView.TabItems.Count(); i++)
            {
                var item = destinationTabView.ContainerFromIndex(i) as TabViewItem;

                if (e.GetPosition(item).X - item.Bounds.Width < 0)
                {
                    index = i;
                    break;
                }
            }

            // Now remove the item from the source TabView
            var srcTabView = tvi.FindAncestorOfType<TabView>();
            var srcIndex = srcTabView.IndexFromContainer(tvi);
            (srcTabView.TabItems as IList).RemoveAt(srcIndex);


            var tab = new TabViewItem
            {
                Header = tvi.Header,
                IconSource = tvi.IconSource,
                Content = new VaultPage()
                {
                    DataContext = (tvi.Content as VaultPage).DataContext
                }
            };
            (tab.Content as VaultPage).DataContext = new VaultPageViewModel();

            var tabCount = destinationTabView.TabItems.Count();
            // Now add it to the new TabView
            if (index < 0 || index == tabCount)
            {
                (destinationTabView.TabItems as IList).Add(tab);
            }
            else if (index < tabCount)
            {
                (destinationTabView.TabItems as IList).Insert(index, tab);
            }

            destinationTabView.SelectedItem = tab;
            e.Handled = true;

            // Remember, TabItemsChanged won't fire during DragDrop so we need to check
            // here if we should close the window if TabItems.Count() == 0
            if (srcTabView.TabItems.Count() == 0)
            {
                var wnd = srcTabView.FindAncestorOfType<TabViewWindowingPopout>(includeSelf: true);
                wnd.Close();
            }
        }
    }

    public void TabStripDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataIdentifier))
        {
            // For dragover, use the standard DragEffects property
            e.DragEffects = DragDropEffects.Move;
        }
    }

    private void TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
    {
        // In this case, the tab was dropped outside of any tabstrip, let's move it to
        // a new window
        var popout = new TabViewWindowingPopout();

        // TabItems is by default initialized to an AvaloniaList<object>, so we can just
        // cast to IList and add
        // Be sure to remove the tab item from it's old TabView FIRST or else you'll get the
        // annoying "Item already has a Visual parent error"
        if (popout.TabView.TabItems is IList l)
        {
            // If you're binding, args also as 'Item' where you can retrieve the data item instead
            (sender.TabItems as IList).Remove(args.Tab);

            // Preserving tab content state is easiest if you aren't binding. If you are, you will
            // need to manage preserving the state of the tabcontent across the different TabViews
            //l.Add(args.Tab);
        }

        // make a shallow copy of the vault page, then add that to the documents of the new VM.
        var vaultPage = new VaultPage();
        var vaultVm = ((args.Tab.Content as VaultPage).DataContext as VaultPageViewModel);
        vaultPage.DataContext = vaultVm;

        //(vaultPage.DataContext as VaultPageViewModel).VaultContents = vaultVm.VaultContents;
        //(vaultPage.DataContext as VaultPageViewModel)._ = vaultVm.VaultContents;
        //(vaultPage.DataContext as VaultPageViewModel).IsCertificatesChecked = vaultVm.IsCertificatesChecked;
        //(vaultPage.DataContext as VaultPageViewModel).IsKeysChecked = vaultVm.IsKeysChecked;
        //(vaultPage.DataContext as VaultPageViewModel).IsSecretsChecked = vaultVm.IsSecretsChecked;
        //(vaultPage.DataContext as VaultPageViewModel).CheckedBoxes = vaultVm.CheckedBoxes;
        //(vaultPage.DataContext as VaultPageViewModel).SearchQuery = vaultVm.SearchQuery;



        var tab = new TabViewItem
        {
            Header = args.Tab.Header,
            IconSource = args.Tab.IconSource,
            Content = vaultPage
            //{
            //    DataContext = new //(args.Tab.Content as VaultPage).DataContext
            //}
        };
        (popout.DataContext as TabViewPageViewModel).Documents.Add(tab);
        
        popout.Width = 800;
        popout.Height = 500;
        popout.Show();

        // TabItemsChanged will fire here and will check if the window is closed, only because it
        // is raised after drag/drop completes, so we don't have to do that here
    }


}