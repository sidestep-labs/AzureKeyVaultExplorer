using System;
using System.Text;

namespace KeyVaultExplorer.Services;

/// <summary>
/// Utility class for formatting PEM files according to RFC 7468 standards
/// </summary>
public static class PemFormatter
{
    /// <summary>
    /// Formats a PEM public key according to RFC 7468 with 64-character line wrapping
    /// </summary>
    /// <param name="publicKey">The public key bytes to format</param>
    /// <returns>A properly formatted PEM string with headers, wrapped base64 content, and footers</returns>
    public static string FormatPublicKey(byte[] publicKey)
    {
        var base64 = Convert.ToBase64String(publicKey);
        var sb = new StringBuilder();
        sb.AppendLine("-----BEGIN PUBLIC KEY-----");
        
        // Wrap base64 content at 64 characters per line as per RFC 7468
        for (int i = 0; i < base64.Length; i += 64)
        {
            int length = Math.Min(64, base64.Length - i);
            sb.AppendLine(base64.Substring(i, length));
        }
        
        sb.Append("-----END PUBLIC KEY-----");
        return sb.ToString();
    }
}
