using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared;
using System.Collections.Generic;
using System.Threading;
using System;

namespace avalonia.kvexplorer.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    public SettingsPageViewModel(AuthService authService)
    {
        _authService = authService;
     
    }

    public SettingsPageViewModel()
    {
        _authService = new AuthService();
    }
    /// <summary>
    /// The Title of this page
    /// </summary>
    public string Title => "Welcome to our Wizard-Sample.";

    /// <summary>
    /// The content of this page
    /// </summary>
    public string Message => "Press \"Next\" to register yourself.";



}