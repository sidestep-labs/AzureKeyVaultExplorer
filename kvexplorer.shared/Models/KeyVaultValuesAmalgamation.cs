using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace kvexplorer.shared.Models;

public class KeyVaultContentsAmalgamation
{
    public KeyVaultItemType Type { get; set; }
    public Uri Id { get; set; }
    public Uri VaultUri { get; set; }
    public Uri ValueUri { get; set; }

    public string Name { get; set; }
    public string Version { get; set; }
    public string ContentType { get; set; }

    public DateTimeOffset? UpdatedOn { get; set; }

    public DateTimeOffset? CreatedOn { get; set; }

    public DateTimeOffset? LastModifiedDate =>  UpdatedOn.HasValue ? UpdatedOn.Value.ToLocalTime() : CreatedOn!.Value.ToLocalTime();

    public string WhenLastModified => GetPrettyDate(LastModifiedDate!.Value.UtcDateTime);

    public SecretProperties SecretProperties { get; set; } = null!;
    public KeyProperties KeyProperties { get; set; } = null!;
    public CertificateProperties? CertificateProperties { get; set; } = null!;

    public IDictionary<string, string> Tags { get; set; } = null!;//new Dictionary<string, string>();

    public string[] TagValues => Tags is not null ? [.. Tags.Values] : [];

    public string[] TagKeys => Tags is not null ? [.. Tags.Keys] : [];

    public string TagValuesString => string.Join(", ", Tags?.Values ?? []);

    private string GetPrettyDate(DateTime d)
    {
        // 1.
        // Get time span elapsed since the date.
        TimeSpan s = DateTime.Now.Subtract(d);

        // 2.
        // Get total number of days elapsed.
        int dayDiff = (int)s.TotalDays;

        // 3.
        // Get total number of seconds elapsed.
        int secDiff = (int)s.TotalSeconds;

        // 4.
        // Don't allow out of range values.
        if (dayDiff < 0 || dayDiff >= 5000)
        {
            return null;
        }

        // 5.
        // Handle same-day times.
        if (dayDiff == 0)
        {
            // A.
            // Less than one minute ago.
            if (secDiff < 60)
            {
                return "just now";
            }
            // B.
            // Less than 2 minutes ago.
            if (secDiff < 120)
            {
                return "1 minute ago";
            }
            // C.
            // Less than one hour ago.
            if (secDiff < 3600)
            {
                return string.Format("{0} minutes ago",
                    Math.Floor((double)secDiff / 60));
            }
            // D.
            // Less than 2 hours ago.
            if (secDiff < 7200)
            {
                return "1 hour ago";
            }
            // E.
            // Less than one day ago.
            if (secDiff < 86400)
            {
                return string.Format("{0} hours ago",
                    Math.Floor((double)secDiff / 3600));
            }
        }
        // 6.
        // Handle previous days.
        if (dayDiff == 1)
        {
            return "yesterday";
        }
        if (dayDiff < 7)
        {
            return string.Format("{0} days ago",dayDiff);
        }
        if (dayDiff < 30)
        {
            var w = Math.Ceiling((double)dayDiff / 7);
            return $"""{w} {(w == 1 ? "week": "weeks")} ago""";
        }

        if (dayDiff > 31)
        {
            var w = Math.Ceiling((double)dayDiff / 30);
            return $"""{w} {(w == 1 ? "month" : "months")} ago""";
        }
        return null;
    }
}

public enum KeyVaultItemType
{
    Certificate,
    Secret,
    Key,
    All
}