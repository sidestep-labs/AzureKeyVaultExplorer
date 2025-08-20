using Avalonia.Controls;
using KeyVaultExplorer.ViewModels;
using System;
using System.ComponentModel;

namespace KeyVaultExplorer;

public partial class EditCertificateVersion : UserControl
{
    public EditCertificateVersion()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if (DataContext is EditCertificateVersionViewModel vm)
        {
            // Wire up the delegate to get tags from the TagsEditor
            vm.GetUpdatedTags = () => TagsEditor.TagsDictionary;
            
            // Subscribe to ViewModel property changes to sync tags
            vm.PropertyChanged += OnViewModelPropertyChanged;
            
            // Initialize tags if KeyVaultCertificateModel is already set
            if (vm.KeyVaultCertificateModel?.Tags != null)
            {
                TagsEditor.TagsDictionary = vm.KeyVaultCertificateModel.Tags;
            }
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EditCertificateVersionViewModel.KeyVaultCertificateModel) && 
            DataContext is EditCertificateVersionViewModel vm)
        {
            // Update TagsEditor when KeyVaultCertificateModel changes
            if (vm.KeyVaultCertificateModel?.Tags != null)
            {
                TagsEditor.TagsDictionary = vm.KeyVaultCertificateModel.Tags;
            }
        }
    }
}