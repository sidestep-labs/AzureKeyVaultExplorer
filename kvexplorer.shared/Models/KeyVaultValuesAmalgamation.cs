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

    public virtual bool? Enabled { get; set; }
    public virtual DateTimeOffset? NotBefore { get; set; }
    public virtual DateTimeOffset? ExpiresOn { get; set; }
    public virtual int? RecoverableDays { get; set; }
    public virtual string? RecoveryLevel { get; set; }

    public DateTimeOffset? LastModifiedDate => UpdatedOn.HasValue ? UpdatedOn.Value.ToLocalTime() : CreatedOn!.Value.ToLocalTime();

    public string WhenLastModified => GetPrettyDate(LastModifiedDate!.Value.UtcDateTime);

    public SecretProperties SecretProperties { get; set; } = null!;
    public KeyProperties KeyProperties { get; set; } = null!;
    public CertificateProperties? CertificateProperties { get; set; } = null!;

    public IDictionary<string, string> Tags { get; set; } = null!;//new Dictionary<string, string>();

    public string[] TagValues => Tags is not null ? [.. Tags.Values] : [];

    public string[] TagKeys => Tags is not null ? [.. Tags.Keys] : [];

    public string TagValuesString => string.Join(", ", Tags?.Values ?? []);

    private string? GetPrettyDate(DateTime d)
    {
        TimeSpan s = DateTime.Now.Subtract(d);
        int dayDiff = (int)s.TotalDays;
        int secDiff = (int)s.TotalSeconds;
        if (dayDiff < 0 || dayDiff >= 5000)
            return null;
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
        if (dayDiff == 1)
            return "yesterday";
        if (dayDiff < 7)
            return string.Format("{0} days ago", dayDiff);
        if (dayDiff < 30)
        {
            var w = Math.Ceiling((double)dayDiff / 7);
            return $"""{w} {(w == 1 ? "week" : "weeks")} ago""";
        }
        if (dayDiff < 366)
        {
            var w = Math.Ceiling((double)dayDiff / 30);
            return $"""{w} {(w == 1 ? "month" : "months")} ago""";
        }
        if (dayDiff > 366)
        {
            var w = Math.Round((double)dayDiff / 365);
            return $"""{w} {(w == 1 ? "year" : "years")} ago""";
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