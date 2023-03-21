using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using avalon.kvexplorer.Services;
using System.Collections.Generic;
using System.Threading;

namespace avalon.kvexplorer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    public MainWindowViewModel(AuthService authService)
    {
        _authService = authService;
        Data = new List<MyData>
        {
            new MyData { Name = "John Doe", Age = 42, Address = "123 Main St." },
            new MyData { Name = "Jane Doe", Age = 39, Address = "456 Oak St." },
            new MyData { Name = "Bob Smith", Age = 27, Address = "789 Elm St." }
        };
    }

    public MainWindowViewModel()
    {
        _authService = new AuthService();
        Data = new List<MyData>
        {
            new MyData { Name = "John Doe", Age = 42, Address = "123 Main St." },
            new MyData { Name = "Jane Doe", Age = 39, Address = "456 Oak St." },
            new MyData { Name = "Bob Smith", Age = 27, Address = "789 Elm St." }
        };
    }

    public class MyData
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }

    [ObservableProperty]
    public List<MyData> _data;


    [RelayCommand]
    private async void Login()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if(account == null)
        {
            await _authService.LoginAsync(cancellation);
        }

    }

    public string Greeting => "Welcome to Avalonia!";
}