using Avalonia.Threading;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.KeyVault;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyVaultExplorer.shared;
using KeyVaultExplorer.shared.Database;
using KeyVaultExplorer.shared.Models;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KeyVaultExplorer.ViewModels;

public partial class ToolBarViewModel : ViewModelBase
{
    [ObservableProperty]
    public bool isBusy;

    public ToolBarViewModel()
    {
    }
}