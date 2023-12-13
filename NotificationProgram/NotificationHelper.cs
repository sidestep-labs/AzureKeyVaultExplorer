#if WINDOWS
using System.Runtime.InteropServices;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace NotificationHelper;

public class NotificationProgram
{
    [ComImport]
    [Guid("53E31837-6600-4A81-9395-75CFFE746F94")]
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface INotificationActivationCallback
    {
        void Activate([MarshalAs(UnmanagedType.LPWStr)] string appUserModelId, [MarshalAs(UnmanagedType.LPWStr)] string invokedArgs, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)][In] byte[] data, int dataCount, [MarshalAs(UnmanagedType.LPWStr)] string tileActivationArguments);
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct NOTIFICATION_USER_INPUT_DATA
    {
        /// <summary>
        /// The key of the user input.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Key;

        /// <summary>
        /// The value of the user input.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Value;

        [ComImport]
        [Guid("53E31837-6600-4A81-9395-75CFFE746F94")]
        [ComVisible(true)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface INotificationActivationCallback
        {
            /// <summary>
            /// The method called when your notification is clicked.
            /// </summary>
            /// <param name="appUserModelId">The app id of the app that sent the toast.</param>
            /// <param name="invokedArgs">The activation arguments from the toast.</param>
            /// <param name="data">The user input from the toast.</param>
            /// <param name="dataCount">The number of user inputs.</param>
            void Activate(
                [In, MarshalAs(UnmanagedType.LPWStr)]
                string appUserModelId,
                [In, MarshalAs(UnmanagedType.LPWStr)]
                string invokedArgs, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] NOTIFICATION_USER_INPUT_DATA[] data,
                [In, MarshalAs(UnmanagedType.U4)]
                uint dataCount);
        }

        [ComImport]
        [Guid("5A7E0885-672A-4B27-8A5D-ACE62403A735")]
        [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
        internal interface IToastNotificationManagerStatics
        {
            ToastNotifierCompat CreateToastNotifier([MarshalAs(UnmanagedType.LPWStr)] string applicationId);
        }

        [ComImport]
        [Guid("FA4E8D69-77E7-4244-9F22-3186E5816A48")]
        [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
        internal interface IToastNotifier
        {
            void Show([In, MarshalAs(UnmanagedType.Interface)] IToastNotification notification);

            void Hide([In, MarshalAs(UnmanagedType.Interface)] IToastNotification notification);
        }

        [ComImport]
        [Guid("04124B20-82C6-4270-951C-8E2B1C8413C2")]
        [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
        internal interface IToastNotification
        {
            void add_Activated([In, MarshalAs(UnmanagedType.Interface)] INotificationActivationCallback activatedHandler);

            void remove_Activated([In, MarshalAs(UnmanagedType.Interface)] INotificationActivationCallback activatedHandler);
        }

        [ComImport]
        [Guid("7B365E70-9DD3-4670-8349-644AF0526294")]
        [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
        internal interface IXmlDocument
        {
        }

        [ComImport]
        [Guid("33E04BCC-1D76-4F3B-A157-4C8142FE91C4")]
        [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
        internal interface IXmlDocumentIO
        {
            void LoadXml([In, MarshalAs(UnmanagedType.HString)] string xml);
        }

        [ComImport]
        [Guid("3C5D3D4F-88A2-4EF5-A3FD-28A4A78F2895")]
        [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
        internal interface IXmlDocumentStatics
        {
            IXmlDocumentIO LoadFromXml([In, MarshalAs(UnmanagedType.HString)] string xml);
        }

        internal class NotificationActivationCallback : INotificationActivationCallback
        {
            public void Activate(string appUserModelId, string invokedArgs, byte[] data, int dataCount)
            {
                // Handle activation
                Console.WriteLine("Toast notification activated!");
            }

            public void Activate([In, MarshalAs(UnmanagedType.LPWStr)] string appUserModelId, [In, MarshalAs(UnmanagedType.LPWStr)] string invokedArgs, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] NOTIFICATION_USER_INPUT_DATA[] data, [In, MarshalAs(UnmanagedType.U4)] uint dataCount)
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif