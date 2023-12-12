using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared;
using kvexplorer.shared.Database;
using kvexplorer.shared.Models;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Collections.Generic;

namespace kvexplorer.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly KvExplorerDb _db;
    //private static Configuration ConfigFile => ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

    [ObservableProperty]
    public string version;

    [ObservableProperty]
    private AuthenticatedUserClaims? authenticatedUserClaims;

    [ObservableProperty]
    private ObservableCollection<Settings> settings;

    [ObservableProperty]
    private bool isBackgroundTransparencyEnabled;

    public SettingsPageViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _db = Defaults.Locator.GetRequiredService<KvExplorerDb>();
        Dispatcher.UIThread.Invoke(async () =>
        {
            Version = GetAppVersion();
            Settings = new ObservableCollection<Settings>(await _db.GetToggleSettings());
            IsBackgroundTransparencyEnabled = Settings.Single(s => s.Name == SettingType.BackgroundTransparency).Value;
        }, DispatcherPriority.Input);
    }

    [RelayCommand]
    private async Task SignInOrRefreshTokenAsync()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);

        if (account is null)
        {
            account = await _authService.LoginAsync(cancellation);
        }

        AuthenticatedUserClaims = new AuthenticatedUserClaims()
        {
            Username = account.Account.Username,
            TenantId = account.TenantId,
            Name = account.ClaimsPrincipal.Identities.First().FindFirst("name").Value,
            Email = account.ClaimsPrincipal.Identities.First().FindFirst("email").Value,
        };
    }

    [RelayCommand]
    private async Task SignOut()
    {
        await _authService.RemoveAccount();
        AuthenticatedUserClaims = null;
    }

    // TODO: Create method of changing the background color from transparent to non stranparent

    [RelayCommand]
    private async Task SetBackgroundColorSetting()
    {
        AddOrUpdateAppSettings("BackgroundTransparency", IsBackgroundTransparencyEnabled);
    }

    //private async Task LoadApplicationVersion()
    //{
    //    //string buildDirProps = Environment.GetEnvironmentVariable("EnvironmentName");
    //    //string _version = await File.ReadAllTextAsync(".\\VERSION.txt");
    //    //if (!System.Version.TryParse(_version, out Version fullVersion))
    //    //{
    //    //    Version = "Missing version file" + buildDirProps;
    //    //    return;
    //    //}
    //    //Version = $"{fullVersion.Major}.{fullVersion.Minor}.{fullVersion.Build}.{fullVersion.Revision}-{buildDirProps}";

    //}

    public static string GetAppVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        return version == null ? "(Unknown)" : $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    public static async Task<Dictionary<string, object>> GetSettings()
    {
        var path = Path.Combine(Constants.LocalAppDataFolder, "settings.json");
        using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(stream);
    }

    public static async Task AddOrUpdateAppSettings(string key, object value)
    {
        var path = Path.Combine(Constants.LocalAppDataFolder, "settings.json");
        var records = await GetSettings();
        records[key] = value;
        var newJson = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, newJson);
    }
}