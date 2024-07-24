using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using KeyVaultExplorer.Exceptions;
using KeyVaultExplorer.ViewModels;
using System;
using System.Threading.Tasks;

namespace KeyVaultExplorer;

public partial class CreateNewSecretVersion : UserControl
{
    public CreateNewSecretVersion()
    {
        InitializeComponent();
        DataContext = new CreateNewSecretVersionViewModel();
    }

    private void InputField_OnAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
    {
        // We will set the focus into our input field just after it got attached to the visual tree.
        if (sender is InputElement inputElement)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                inputElement.Focus(NavigationMethod.Unspecified, KeyModifiers.None);
            });
        }
    }
}