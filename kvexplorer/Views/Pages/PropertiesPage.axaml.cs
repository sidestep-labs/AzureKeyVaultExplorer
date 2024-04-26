using Avalonia.Controls;
using kvexplorer.shared;
using kvexplorer.ViewModels;
using System.Collections.Generic;
using System;

namespace kvexplorer;

public partial class PropertiesPage : UserControl
{

    public PropertiesPage()
    {
        InitializeComponent();
        DataContext = new PropertiesPageViewModel();
    }
}