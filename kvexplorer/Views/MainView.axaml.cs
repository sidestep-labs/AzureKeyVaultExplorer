using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using kvexplorer.ViewModels;
using kvexplorer.Views.Pages;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace kvexplorer.Views;

public partial class MainView : UserControl
{
    private Frame? _frameView;
    private NavigationView? _navView;
    public static MainView? Instance { get; private set; }
    public static readonly RoutedEvent<RoutedEventArgs> NavigateHomeEvent = RoutedEvent.Register<MainView, RoutedEventArgs>(nameof(NavigateHomeEvent), RoutingStrategies.Tunnel);

    public MainView()
    {
        Instance = this;
        InitializeComponent();
        KeyUp += TabViewPage_KeyUpFocusSearchBox;

        var treeVaultVm = Defaults.Locator.GetRequiredService<KeyVaultTreeListViewModel>();
        var mainViewModel = Defaults.Locator.GetRequiredService<MainViewModel>();

        Dispatcher.UIThread.Post(async () =>
        {
            await mainViewModel.RefreshTokenAndGetAccountInformation().ContinueWith(async (t) =>
            {
                await treeVaultVm.GetAvailableKeyVaultsCommand.ExecuteAsync(false);
            });
        }, DispatcherPriority.MaxValue);

        AddHandler(NavigateHomeEvent, OnNavigateHomeEvent, RoutingStrategies.Tunnel, handledEventsToo: false);
    }

    private void OnNavigateHomeEvent(object sender, RoutedEventArgs e)
    {
        if (FrameView.Content.GetType().Name != nameof(MainPage))
            FrameView.NavigateFromObject(new MainPage());

        return;
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

        //NavView.MenuItemsSource = navMenuItems.Prepend(navViewItems.First());
        NavView.FooterMenuItemsSource = footerItems;

        //NavView.MenuItemsSource = GetNavigationViewItems();
        //NavView.FooterMenuItemsSource = GetFooterNavigationViewItems();
        NavView.IsPaneOpen = true;

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
            MainPage => this.TryFindResource(selected ? "LibraryIcon" : "LibraryIcon", out var value) ? (IconSource)value! : null,
            SubscriptionsPageViewModel => this.TryFindResource(selected ? "Bookmarks" : "Bookmarks", out var value) ? (IconSource)value! : null,
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
                Content = "Vault Library",
                Tag = NavigationFactory.GetPages()[0],
                IconSource= this.FindResource("LibraryIcon") as IconSource
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

    private void ShowAccountTeachingTip(object sender, TappedEventArgs e)
    {
        AccountTeachingTip.IsOpen = true;
    }

    private void TeachingTip_ActionButtonClick(TeachingTip sender, System.EventArgs args)
    {
        if (FrameView.Content.GetType().FullName == "kvexplorer.Views.Pages.SettingsPage")
            return;
        FrameView.NavigateFromObject(new SettingsPage());
    }


    private void TabViewPage_KeyUpFocusSearchBox(object sender, KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.F && (e.KeyModifiers == KeyModifiers.Control || e.Key == Avalonia.Input.Key.LWin || e.Key == Avalonia.Input.Key.RWin))
        {
            var vvpage = this.FindDescendantOfType<VaultPage>();
            vvpage?.FindControl<TextBox>("SearchTextBox")?.Focus();
        }
    }

}