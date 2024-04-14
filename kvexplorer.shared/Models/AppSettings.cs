namespace kvexplorer.shared.Models;
using System.ComponentModel.DataAnnotations;

public class AppSettings
{
    public bool BackgroundTransparency { get; set; }
    public int ClipboardTimeout { get; set; }

    [AllowedValues("Left", "Auto", "Top")]
    public string NavigationLayoutMode { get; set; }

    [AllowedValues("System", "Light", "Dark")]
    public string AppTheme { get; set; }
}