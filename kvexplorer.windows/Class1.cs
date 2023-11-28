using Microsoft.Toolkit.Uwp.Notifications;

namespace kvexplorer.windows;

public class Class1
{



    public void ShowNotification()
    {

        // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
        var toast = new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddText("Andrew sent you a picture").AddText("Check this out, The Enchantments in Washington!");
        toast.Show();

    }
}
