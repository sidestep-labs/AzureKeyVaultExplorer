using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KeyVaultExplorer.Views.CustomControls;

public partial class TagsEditor : UserControl
{
    private ObservableCollection<TagItem> _tags = new();

    public TagsEditor()
    {
        InitializeComponent();
        DataContext = this;
    }

    public ObservableCollection<TagItem> Tags
    {
        get => _tags;
        set => _tags = value;
    }

    public IDictionary<string, string> TagsDictionary
    {
        get
        {
            var dict = new Dictionary<string, string>();
            foreach (var tag in Tags)
            {
                if (!string.IsNullOrWhiteSpace(tag.Key) && !string.IsNullOrWhiteSpace(tag.Value))
                {
                    dict[tag.Key] = tag.Value;
                }
            }
            return dict;
        }
        set
        {
            Tags.Clear();
            if (value != null)
            {
                foreach (var kvp in value)
                {
                    Tags.Add(new TagItem { Key = kvp.Key, Value = kvp.Value });
                }
            }
        }
    }

    private void AddTag_Click(object? sender, RoutedEventArgs e)
    {
        Tags.Add(new TagItem { Key = "", Value = "" });
    }

    private void RemoveTag_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is TagItem tagItem)
        {
            Tags.Remove(tagItem);
        }
    }
}

public class TagItem : INotifyPropertyChanged
{
    private string _key = "";
    private string _value = "";

    public string Key
    {
        get => _key;
        set
        {
            _key = value;
            OnPropertyChanged();
        }
    }

    public string Value
    {
        get => _value;
        set
        {
            _value = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}