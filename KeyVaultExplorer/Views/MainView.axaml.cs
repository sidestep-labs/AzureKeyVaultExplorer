using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Navigation;
using KeyVaultExplorer.ViewModels;
using KeyVaultExplorer.Views.Pages;
using System;

#nullable disable

namespace KeyVaultExplorer.Views;

public partial class MainView : UserControl
{
    public static MainView? Instance { get; private set; }
    public static readonly RoutedEvent<RoutedEventArgs> NavigateHomeEvent = RoutedEvent.Register<MainView, RoutedEventArgs>(nameof(NavigateHomeEvent), RoutingStrategies.Tunnel);
    public static readonly RoutedEvent<RoutedEventArgs> NavigateSettingsEvent = RoutedEvent.Register<MainView, RoutedEventArgs>(nameof(NavigateSettingsEvent), RoutingStrategies.Tunnel);
    public static readonly RoutedEvent<RoutedEventArgs> NavigateSubscriptionsEvent = RoutedEvent.Register<MainView, RoutedEventArgs>(nameof(NavigateSubscriptionsEvent), RoutingStrategies.Tunnel);

    public MainView()
    {
        Instance = this;
        InitializeComponent();
        KeyUp += TabViewPage_KeyUpFocusSearchBox;
        //NavView.BackRequested += OnNavigationViewBackRequested;
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
        AddHandler(NavigateSettingsEvent, OnNavigateSettingsEvent, RoutingStrategies.Tunnel, handledEventsToo: false);
        AddHandler(NavigateSubscriptionsEvent, OnNavigateSubscriptionsEvent, RoutingStrategies.Tunnel, handledEventsToo: false);
    }

    private void OnNavigationViewBackRequested(object sender, NavigationViewBackRequestedEventArgs e)
    {
        FrameView.GoBack();
    }

    private void OnNavigateHomeEvent(object sender, RoutedEventArgs e)
    {
        if (FrameView.Content.GetType().Name != nameof(MainPage))
            FrameView.NavigateFromObject(new MainPage(), NavOptions);
    }

    private void OnNavigateSettingsEvent(object sender, RoutedEventArgs e)
    {
        if (FrameView.Content.GetType().Name != nameof(SettingsPage))
            FrameView.NavigateFromObject(new SettingsPage(), NavOptions);
    }

    private void OnNavigateSubscriptionsEvent(object sender, RoutedEventArgs e)
    {
        if (FrameView.Content.GetType().Name != nameof(SubscriptionsPage))
            FrameView.NavigateFromObject(new SubscriptionsPage(), NavOptions);
    }

    //var menuItems = new List<NavigationViewItemBase>(4);
    //var footerItems = new List<NavigationViewItemBase>(2);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var vm = Defaults.Locator.GetRequiredService<MainViewModel>();

        DataContext = vm;
        FrameView.NavigationPageFactory = vm.NavigationFactory;

        FrameView.Navigated += OnFrameViewNavigated;

        FrameView.NavigateFromObject(new MainPage());
    }

    private void OnFrameViewNavigated(object sender, NavigationEventArgs e)
    {
        var page = e.Content as Control;

        if (FrameView.BackStackDepth > 0) //&& !NavView.IsBackButtonVisible
        {
            AnimateContentForBackButton(true);
        }
        else if (FrameView.BackStackDepth == 0) // && NavView.IsBackButtonVisible
        {
            AnimateContentForBackButton(false);
        }
    }

    private async void AnimateContentForBackButton(bool show)
    {
        if (!WindowIcon.IsVisible)
            return;

        if (show)
        {
            var ani = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(250),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(12, 12, 12, 4))
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        KeySpline = new KeySpline(0,0,0,1),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(48,12,12,4))
                        }
                    }
                }
            };

            await ani.RunAsync(WindowIcon);

            //NavView.IsBackButtonVisible = true;
        }
        else
        {
            //NavView.IsBackButtonVisible = false;

            var ani = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(250),
                FillMode = FillMode.Forward,

                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(48,12,12,4))
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        KeySpline = new KeySpline(0,0,0,1),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(12, 12, 12, 4))
                        }
                    }
                }
            };
            await ani.RunAsync(WindowIcon);
        }
    }

    private FrameNavigationOptions NavOptions => new FrameNavigationOptions
    {
        TransitionInfoOverride = new SlideNavigationTransitionInfo(),
        IsNavigationStackEnabled = true
    };

    private void OnNavigationViewItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItemContainer is NavigationViewItem { Tag: Control c })
        {
            _ = FrameView.NavigateFromObject(c, NavOptions);
        }
    }

    //private void ShowAccountTeachingTip(object sender, TappedEventArgs e)
    //{
    //    AccountTeachingTip.IsOpen = true;
    //}

    //private void TeachingTip_ActionButtonClick(TeachingTip sender, System.EventArgs args)
    //{
    //    if (FrameView.Content.GetType().Name == nameof(SettingsPage))
    //        return;
    //    FrameView.NavigateFromObject(new SettingsPage(), NavOptions);
    //}

    private void TabViewPage_KeyUpFocusSearchBox(object sender, KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.F && (e.KeyModifiers == KeyModifiers.Control || e.Key == Avalonia.Input.Key.LWin || e.Key == Avalonia.Input.Key.RWin))
        {
            var vvpage = this.FindDescendantOfType<VaultPage>();
            vvpage?.FindControl<TextBox>("SearchTextBox")?.Focus();
        }
    }
}