using avalonia.kvexplorer.ViewModels;
using avalonia.kvexplorer.Views.Pages;
using Avalonia;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace avalonia.kvexplorer.Views;

public partial class MainView : UserControl
{
    private Frame? _frameView;
    private NavigationView? _navView;
    public static MainView? Instance { get; private set; }

    public MainView()
    {
        Instance = this;
        InitializeComponent();
    }

    //var menuItems = new List<NavigationViewItemBase>(4);
    //var footerItems = new List<NavigationViewItemBase>(2);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var vm = Defaults.Locator.GetRequiredService<MainViewModel>();
        _navView = this.FindControl<NavigationView>("NavView");
        var navViewItems = _navView.MenuItems;
        var navMenuItems = navViewItems.TakeLast(2).Cast<NavigationViewItem>();
        var footerItems = _navView.FooterMenuItems.Cast<NavigationViewItem>();

        DataContext = vm;
        FrameView.NavigationPageFactory = vm.NavigationFactory;

        FrameView.Navigated += OnFrameViewNavigated;
        NavView.ItemInvoked += OnNavigationViewItemInvoked;
       

        //  for (var i = 0; i < nv.FooterMenuItems.Count; i++)
        //  {
        //      ((NavigationViewItem)nv.FooterMenuItems[i]).Tag = NavigationFactory.GetPages().Last();
        //  }

        //todo remove
        var pages = NavigationFactory.GetPages();
        navMenuItems.ElementAt(0).Tag = pages[0];
        navMenuItems.ElementAt(1).Tag = pages[1];
        footerItems.ElementAt(0).Tag = pages[2];

        
        NavView.MenuItemsSource = navMenuItems.Prepend(navViewItems.First());
        NavView.FooterMenuItemsSource = footerItems;

        //NavView.MenuItemsSource = GetNavigationViewItems();
        //NavView.FooterMenuItemsSource = GetFooterNavigationViewItems();
        NavView.IsPaneOpen = false;

        //FrameView.NavigateFromObject(navViewItems.ElementAt(1).Tag);
        FrameView.NavigateFromObject(new MainPage());
    }

    private void SetNVIIcon(NavigationViewItem? item, bool selected)
    {
        // Technically, yes you could set up binding and converters and whatnot to let the icon change
        // between filled and unfilled based on selection, but this is so much simpler

        if (item == null)
            return;

        var t = item.Tag;

        item.IconSource = t switch
        {
            MainPage => this.TryFindResource(selected ? "HomeIcon" : "HomeIcon", out var value) ? (IconSource)value! : null,
            BookmarksPageViewModel => this.TryFindResource(selected ? "Bookmarks" : "Bookmarks", out var value) ? (IconSource)value! : null,
            SettingsPage => this.TryFindResource(selected ? "SettingsIcon" : "SettingsIcon", out var value) ? (IconSource)value! : null,
            _ => item.IconSource
        };
    }

    private void OnFrameViewNavigated(object sender, NavigationEventArgs e)
    {
        var page = e.Content as Control;

        foreach (NavigationViewItem nvi in NavView.MenuItems.TakeLast(2))
        {
            if (nvi.Tag != null && nvi.Tag.Equals(page))
            {
                NavView.SelectedItem = nvi;
                SetNVIIcon(nvi, true);
            }
            else
            {
                SetNVIIcon(nvi, false);
            }
        }

        foreach (NavigationViewItem nvi in NavView.FooterMenuItemsSource)
        {
            if (nvi.Tag != null && nvi.Tag.Equals(page))
            {
                NavView.SelectedItem = nvi;
                SetNVIIcon(nvi, true);
            }
            else
            {
                SetNVIIcon(nvi, false);
            }
        }
    }





    private IEnumerable<NavigationViewItem> GetNavigationViewItems()
    {
        return new List<NavigationViewItem>
        {
            new()
            {
                Content = "Home",
                Tag = NavigationFactory.GetPages()[0],
                IconSource= this.FindResource("HomeIcon") as IconSource
            }
        };
    }

    private IEnumerable<NavigationViewItem> GetFooterNavigationViewItems()
    {
        return new List<NavigationViewItem>
        {
            new()
            {
                Content = "Settings",
                Tag = NavigationFactory.GetPages()[2],
                IconSource= this.FindResource("SettingsIcon") as IconSource
            }
        };
    }

    private void OnNavigationViewItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        SetNVIIcon((_navView!.SelectedItem as NavigationViewItem)!, false);

        if (e.InvokedItemContainer is NavigationViewItem { Tag: Control c })
        {
            _ = FrameView.NavigateFromObject(c);
        }
    }
}