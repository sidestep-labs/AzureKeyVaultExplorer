using Avalonia.Controls;
using KeyVaultExplorer.ViewModels;
using System;
using System.ComponentModel;

namespace KeyVaultExplorer;

public partial class EditKeyVersion : UserControl
{
    public EditKeyVersion()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if (DataContext is EditKeyVersionViewModel vm)
        {
            // Wire up the delegate to get tags from the TagsEditor
            vm.GetUpdatedTags = () => TagsEditor.TagsDictionary;
            
            // Subscribe to ViewModel property changes to sync tags
            vm.PropertyChanged += OnViewModelPropertyChanged;
            
            // Initialize tags if KeyVaultKeyModel is already set
            if (vm.KeyVaultKeyModel?.Tags != null)
            {
                TagsEditor.TagsDictionary = vm.KeyVaultKeyModel.Tags;
            }
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EditKeyVersionViewModel.KeyVaultKeyModel) && 
            DataContext is EditKeyVersionViewModel vm)
        {
            // Update TagsEditor when KeyVaultKeyModel changes
            if (vm.KeyVaultKeyModel?.Tags != null)
            {
                TagsEditor.TagsDictionary = vm.KeyVaultKeyModel.Tags;
            }
        }
    }
}