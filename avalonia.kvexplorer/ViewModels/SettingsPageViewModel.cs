using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared;
using System.Collections.Generic;
using System.Threading;
using System;

namespace avalonia.kvexplorer.ViewModels;

public partial class SettingsPageViewModel : PageViewModelBase
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

    // This is our first page, so we can navigate to the next page in any case
    public override bool CanNavigateNext
    {
        get => true;
        protected set => throw new NotSupportedException();
    }

    // You cannot go back from this page
    public override bool CanNavigatePrevious
    {
        get => false;
        protected set => throw new NotSupportedException();
    }



}