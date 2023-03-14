using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using kvexplorer_av.Services;
using kvexplorer_av.ViewModels;
using kvexplorer_av.Views;

namespace kvexplorer_av
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            //initialize dependencies 
            var authService = new AuthService();
            var mainViewModel = new MainWindowViewModel(authService);
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                   
                };

            }


            base.OnFrameworkInitializationCompleted();
        }
    }
}