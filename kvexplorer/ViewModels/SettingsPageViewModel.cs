using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Database;
using kvexplorer.shared.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace kvexplorer.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly KvExplorerDb _db;
    private const string BackgroundTranparency = "BackgroundTransparency";
    //private static Configuration ConfigFile => ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

    [ObservableProperty]
    public string version;

    [ObservableProperty]
    private AuthenticatedUserClaims? authenticatedUserClaims;

    [ObservableProperty]
    private ObservableCollection<Settings> settings;

    [ObservableProperty]
    private bool isBackgroundTransparencyEnabled;

    [ObservableProperty]
    private string navigationLayoutMode;

    [ObservableProperty]
    private int clearClipboardTimeout = 30;

    public List<string> Items => ["Auto", "Left", "Top"];

    public SettingsPageViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _db = Defaults.Locator.GetRequiredService<KvExplorerDb>();
        Dispatcher.UIThread.Invoke(async () =>
        {
            Version = GetAppVersion();
            var s = await _db.GetToggleSettings();
            ClearClipboardTimeout = s.ClipboardTimeout;
            IsBackgroundTransparencyEnabled = (await GetAppSettings()).BackgroundTransparency;
            NavigationLayoutMode = s.NavigationLayoutMode;
        }, DispatcherPriority.MaxValue);
    }

    [RelayCommand]
    private async Task SignInOrRefreshTokenAsync()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);

        if (account is null)
            account = await _authService.LoginAsync(cancellation);

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
        await AddOrUpdateAppSettings(BackgroundTranparency, IsBackgroundTransparencyEnabled);
    }

    [RelayCommand]
    private async Task SetNavigationLayout()
    {
        await AddOrUpdateAppSettings(nameof(NavigationLayoutMode), NavigationLayoutMode);
    }

    [RelayCommand]
    private async Task SetClearClipboardTimeout()
    {
        await Task.Delay(50); // TOOD: figure out a way to get the value without having to wait for it to propagate.
        await _db.UpdateToggleSettings(SettingType.ClipboardTimeout, ClearClipboardTimeout);
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

    public async Task<AppSettings> GetAppSettings()
    {
        var path = Path.Combine(Constants.LocalAppDataFolder, "settings.json");
        using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<AppSettings>(stream);
    }

    public async Task AddOrUpdateAppSettings(string key, bool value)
    {
        var path = Path.Combine(Constants.LocalAppDataFolder, "settings.json");
        var records = await GetAppSettings();
        records.BackgroundTransparency = value;
        var newJson = JsonSerializer.Serialize(records);
        await File.WriteAllTextAsync(path, newJson);
    }

    public async Task AddOrUpdateAppSettings<T>(string key, T value)
    {
        var path = Path.Combine(Constants.LocalAppDataFolder, "settings.json");
        var records = await GetAppSettings();
        // Assuming records is a class with a property that matches the key
        var property = records.GetType().GetProperty(key);
        if (property != null && property.PropertyType == typeof(T))
        {
            property.SetValue(records, value);
            var newJson = JsonSerializer.Serialize(records);
            await File.WriteAllTextAsync(path, newJson);
        }
    }
}