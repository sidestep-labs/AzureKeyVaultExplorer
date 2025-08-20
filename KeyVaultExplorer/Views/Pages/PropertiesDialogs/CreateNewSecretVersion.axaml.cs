using Avalonia.Controls;
using KeyVaultExplorer.ViewModels;
using System.ComponentModel;

namespace KeyVaultExplorer;

public partial class CreateNewSecretVersion : UserControl
{
    public CreateNewSecretVersion()
    {
        InitializeComponent();
        var vm = new CreateNewSecretVersionViewModel();
        DataContext = vm;
        
        // Wire up the delegate to get tags from the TagsEditor
        vm.GetUpdatedTags = () => TagsEditor.TagsDictionary;
        
        // Subscribe to ViewModel property changes to sync tags
        vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CreateNewSecretVersionViewModel.KeyVaultSecretModel) && 
            DataContext is CreateNewSecretVersionViewModel vm)
        {
            // Update TagsEditor when KeyVaultSecretModel changes
            if (vm.KeyVaultSecretModel?.Tags != null)
            {
                TagsEditor.TagsDictionary = vm.KeyVaultSecretModel.Tags;
            }
        }
    }

    //private void Subscription_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    //{
    //    var item = (sender as AutoCompleteBox)!.SelectedItem as SubscriptionDataItem;
    //    (DataContext as CreateNewSecretVersionViewModel)!.SelectedSubscriptionChangedCommand.Execute(item);
    //}
}