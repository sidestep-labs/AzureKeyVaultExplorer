using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using KeyVaultExplorer.Exceptions;
using KeyVaultExplorer.Models;
using KeyVaultExplorer.ViewModels;
using System;
using System.Threading.Tasks;

namespace KeyVaultExplorer;

public partial class CreateNewSecretVersion : UserControl
{
    public CreateNewSecretVersion()
    {
        InitializeComponent();
        var vm = new CreateNewSecretVersionViewModel();
        DataContext = vm;
    }



    //private void Subscription_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    //{
    //    var item = (sender as AutoCompleteBox)!.SelectedItem as SubscriptionDataItem;
    //    (DataContext as CreateNewSecretVersionViewModel)!.SelectedSubscriptionChangedCommand.Execute(item);
    //}
}