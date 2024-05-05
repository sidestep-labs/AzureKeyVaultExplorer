using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.ResourceManager.KeyVault;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using KeyVaultExplorer.Models;
using KeyVaultExplorer.ViewModels;
using System.Linq;

namespace KeyVaultExplorer.Views.CustomControls;

public partial class KeyVaultTreeList : UserControl
{
    private readonly TabViewPageViewModel _tabViewViewModel;
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<KeyVaultTreeList, string>(nameof(Title), defaultValue: "test");
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public KeyVaultTreeList()
    {
        InitializeComponent();
        DataContext = Defaults.Locator.GetRequiredService<KeyVaultTreeListViewModel>();
        _tabViewViewModel = Defaults.Locator.GetRequiredService<TabViewPageViewModel>();
        SubscriptionTreeViewList = this.FindControl<TreeView>("SubscriptionTreeViewList")!;
        SubscriptionTreeViewList.ContextRequested += OnDataGridRowContextRequested;
    }

    private void OnDataGridRowContextRequested(object sender, ContextRequestedEventArgs e)
    {
        var tv = sender as TreeView;
        if (tv.SelectedItem is not null)
        {
            var kvm = tv.ItemsSource.ElementAt(0) as KvSubscriptionModel;
            var showUnpin = kvm.ResourceGroups[0].KeyVaultResources.Contains(tv.SelectedItem as KeyVaultResource);
            ShowMenu(isTransient: true, isCurrentlyPinned: showUnpin);
        }
        e.Handled = true;
    }

    private void ShowMenu(bool isTransient, bool isCurrentlyPinned)
    {
        var flyout = Resources["FAMenuFlyoutSubscriptionTreeView"] as FAMenuFlyout;

        // if pinned, enable unpinned item
        foreach (MenuFlyoutItem item in flyout.Items)
        {
            _ = item.Name switch
            {
                "PinToQuickAccess" => item.IsVisible = !isCurrentlyPinned,
                "UnpinFromQuickAccess" => item.IsVisible = isCurrentlyPinned,
                _ => item.IsVisible = true
            };
        }
        //var unpinnedItem = (MenuFlyoutItem)flyout.Items.ElementAt(1);
        //unpinnedItem.
        /*
                 <ui:MenuFlyoutItem
                Name="PinToQuickAccess"
                Command="{Binding PinVaultToQuickAccessCommand}"
                CommandParameter="{Binding #SubscriptionTreeViewList.SelectedItem}"
                IconSource="OutlineStar"
                Text="Pin to Quick Access" />*/
        //var unpinItem = new MenuFlyoutItem()
        //{
        //    Name = "UnPinToQuickAccess",
        //    IsEnabled = false,
        //    Text = "Remove From Quick Access",
        //    IconSource = new SymbolIconSource { Symbol = Symbol.StarOff },
        //    Command = (DataContext as KeyVaultTreeListViewModel).PinVaultToQuickAccessCommand
        //};
        //flyout.Items.Add(unpinItem);
        flyout.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
        var loc = this.FindControl<TreeView>("SubscriptionTreeViewList");
        flyout.ShowAt(loc);
    }

    private void RefreshKeyVaultList(object sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(async () =>
        {
            await (DataContext as KeyVaultTreeListViewModel)!.GetAvailableKeyVaultsCommand.ExecuteAsync(true);
        }, DispatcherPriority.Input);
    }

    private void OnDoubleClicked(object sender, TappedEventArgs args)
    {
        var sx = (TreeView)sender!;
        if (sx.SelectedItem is KeyVaultResource model)
        {
            Dispatcher.UIThread.Post(() =>
            {
                _tabViewViewModel.AddVaultPageCommand.Execute(model.Data);
            }, DispatcherPriority.ContextIdle);
        }
        Control control = (Control)sender!;
        control.RaiseEvent(new RoutedEventArgs(MainView.NavigateHomeEvent));
    }

    private void TreeListFlyoutItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var sx = (MenuFlyoutItem)sender!;

        if (sx.DataContext is KeyVaultResource)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var model = (KeyVaultTreeListViewModel)DataContext;

                _tabViewViewModel.AddVaultPageCommand.Execute(model.SelectedTreeItem.Data);
            }, DispatcherPriority.ContextIdle);
        }
        Control control = (Control)sender!;
        control.RaiseEvent(new RoutedEventArgs(MainView.NavigateHomeEvent));
    }
}