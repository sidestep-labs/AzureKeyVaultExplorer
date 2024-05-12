using System;
using System.Runtime.InteropServices;

namespace KeyVaultExplorer;

public static class MacOSKeyChainService
{
    public const string SecurityLibrary = "/System/Library/Frameworks/Security.framework/Security";

    [DllImport(SecurityLibrary)]
    public static extern int SecKeychainAddGenericPassword(
        IntPtr keychain,
        uint serviceNameLength,
        string serviceName,
        uint accountNameLength,
        string accountName,
        uint passwordLength,
        IntPtr passwordData,
        IntPtr itemRef);

    [DllImport(SecurityLibrary)]
    private static extern int SecKeychainItemFreeContent(
          IntPtr attrList,
          IntPtr data);

    [DllImport(SecurityLibrary)]
    private static extern int SecKeychainFindGenericPassword(
        IntPtr keychainOrArray,
        uint serviceNameLength,
        string serviceName,
        uint accountNameLength,
        string accountName,
        out uint passwordLength,
        out IntPtr passwordData,
        IntPtr itemRef);

    public static void SaveToKeychain(string serviceName, string accountName, string password)
    {
        IntPtr itemRef = IntPtr.Zero;
        try
        {
            var result = SecKeychainAddGenericPassword(
                IntPtr.Zero,
                (uint)serviceName.Length, serviceName,
                (uint)accountName.Length, accountName,
                (uint)password.Length, Marshal.StringToCoTaskMemAnsi(password),
                itemRef);

            if (result != 0)
            {
                throw new Exception($"Failed to save to Keychain. Error: {result}");
            }
        }
        finally
        {
            if (itemRef != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(itemRef);
            }
        }
    }

    public static string GetPassword(string serviceName, string accountName)
    {
        IntPtr itemRef = IntPtr.Zero;
        IntPtr passwordData = IntPtr.Zero;

        try
        {
            uint passwordLength;
            int result = SecKeychainFindGenericPassword(
                IntPtr.Zero,
                (uint)serviceName.Length, serviceName,
                (uint)accountName.Length, accountName,
                out passwordLength,
                out passwordData,
                itemRef);

            if (result != 0)
            {
                throw new Exception($"Failed to retrieve password from Keychain. Error: {result}");
            }

            if (passwordData != IntPtr.Zero)
            {
                byte[] passwordBytes = new byte[passwordLength];
                Marshal.Copy(passwordData, passwordBytes, 0, (int)passwordLength);

                return System.Text.Encoding.UTF8.GetString(passwordBytes);
            }
            else
            {
                throw new Exception("No password data found in Keychain.");
            }
        }
        finally
        {
            if (itemRef != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(itemRef);
            }

            if (passwordData != IntPtr.Zero)
            {
                SecKeychainItemFreeContent(IntPtr.Zero, passwordData);
            }
        }
    }
}