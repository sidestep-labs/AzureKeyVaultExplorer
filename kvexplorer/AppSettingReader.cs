using Avalonia.Threading;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace kvexplorer;

public class AppSettingReader
{
    public AppSettings AppSettings { get; set; }
    private string path => Path.Combine(Constants.LocalAppDataFolder, "settings.json");

    public AppSettingReader()
    {
        using var stream = File.OpenRead(path);
        var settings =  JsonSerializer.Deserialize<AppSettings>(stream);
        AppSettings = settings;
    }
}