using Avalonia;
using Avalonia.Controls.Primitives;

namespace KeyVaultExplorer.Views.CustomControls;

public class CustomTitleBar : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty =
   AvaloniaProperty.Register<CustomTitleBar, string>(nameof(Title), defaultValue: "Azure Key Vault Explorer");

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}