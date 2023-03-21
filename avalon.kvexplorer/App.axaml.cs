using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using avalon.kvexplorer.Services;
using avalon.kvexplorer.ViewModels;
using avalon.kvexplorer.Views;

namespace avalon.kvexplorer
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