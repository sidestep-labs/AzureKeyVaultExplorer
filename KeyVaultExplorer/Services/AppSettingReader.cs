using Avalonia.Threading;
using KeyVaultExplorer.Models;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Text.Json.Serialization.Metadata;
using KeyVaultExplorer.Models;

namespace KeyVaultExplorer.Services;

public class AppSettingReader
{
    public AppSettings AppSettings { get; set; }
    private string path => Path.Combine(Constants.LocalAppDataFolder, "settings.json");

    public AppSettingReader()
    {
        using var stream = File.OpenRead(path);

        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };
        var settings = JsonSerializer.Deserialize<AppSettings>(stream, options);
        AppSettings = settings;
    }
}