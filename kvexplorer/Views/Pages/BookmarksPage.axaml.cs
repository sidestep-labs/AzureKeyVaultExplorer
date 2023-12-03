using kvexplorer.ViewModels;
using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using NotificationHelper;

#if WINDOWS
using NotificationHelper;
using System.Runtime.InteropServices;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
#endif

namespace kvexplorer.Views.Pages;

public partial class BookmarksPage : UserControl
{
    public BookmarksPage()
    {
        InitializeComponent();
        DataContext = new BookmarksPageViewModel();
    }

    public string GenerateGuid(string name)
    {
        byte[] buf = Encoding.UTF8.GetBytes(name);
        byte[] guid = new byte[16];
        if (buf.Length < 16)
        {
            Array.Copy(buf, guid, buf.Length);
        }
        else
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(buf);

                // Hash is 20 bytes, but we need 16. We loose some of "uniqueness", but I doubt it will be fatal
                Array.Copy(hash, guid, 16);
            }
        }

        // Don't use Guid constructor, it tends to swap bytes. We want to preserve original string as hex dump.
        string guidS = $"{guid[0]:X2}{guid[1]:X2}{guid[2]:X2}{guid[3]:X2}-{guid[4]:X2}{guid[5]:X2}-{guid[6]:X2}{guid[7]:X2}-{guid[8]:X2}{guid[9]:X2}-{guid[10]:X2}{guid[11]:X2}{guid[12]:X2}{guid[13]:X2}{guid[14]:X2}{guid[15]:X2}";

        return guidS;
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
#if WINDOWS
        var process= Process.GetCurrentProcess();
        Console.WriteLine($"Process ID: {process.Id}");
        Console.WriteLine($"Full Path: {process.MainModule.FileName}");
        var toastNotifier = ToastNotificationManagerCompat.CreateToastNotifier(" ");
        string aumid = process.Id.ToString();
        string conciseAumid = aumid.Contains('\\') || aumid.Contains('/') ? GenerateGuid(aumid) :aumid;

        var context = WindowsApplicationContext.FromCurrentProcess();

        var toastNotifier2 = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier(context.AppUserModelId);

        string toastXml = """
          <toast activationType="protocol"> // protocol,Background,Foreground

            <visual>
            <binding template='ToastGeneric'>
                <text id='1'>Hello, World!</text>
            <image placement="appLogoOverride" src="C:/repos/sidestep/kvexplorer.Desktop/kv-noborder.ico"/>
            </binding>
            </visual>
            </toast>
        """;

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(toastXml);
        ToastNotification toast = new ToastNotification(doc);
        toast.ExpirationTime = DateTimeOffset.Now + TimeSpan.FromSeconds(10);

        //toastNotifier.Show(toast);
        toastNotifier2.Show(toast);
        Console.WriteLine("Press any key to exit...");
#endif
    }
}