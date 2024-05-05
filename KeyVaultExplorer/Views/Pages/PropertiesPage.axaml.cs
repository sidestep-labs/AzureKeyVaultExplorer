using Avalonia.Controls;
using KeyVaultExplorer.shared;
using KeyVaultExplorer.ViewModels;
using System.Collections.Generic;
using System;

namespace KeyVaultExplorer;

public partial class PropertiesPage : UserControl
{

    public PropertiesPage()
    {
        InitializeComponent();
        DataContext = new PropertiesPageViewModel();
    }
}