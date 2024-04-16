using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using kvexplorer.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace kvexplorer.ViewModels;

public partial class MainPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public string email;


    public MainPageViewModel()
    {
       
    }

}
