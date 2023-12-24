namespace kvexplorer.shared.Database;

public enum SettingType
{
    BackgroundTransparency
}

public class Settings
{
    public SettingType Name { get; set; }
    public bool Value { get; set; }
}