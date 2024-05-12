using KeyVaultExplorer.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Avalonia.Animation;
using System.Data.Common;
using KeyVaultExplorer.Services;

namespace KeyVaultExplorer.Database;

public partial class KvExplorerDb : IDisposable
{
    private static DbConnection _connection;

    public KvExplorerDb()
    {
    }

    public static void OpenSqlConnection()
    {
        string DataSource = Path.Combine(Constants.LocalAppDataFolder, "KeyVaultExplorer.db");
        var pass =  DatabaseEncryptedPasswordManager.GetSecret();
        var connection = new SqliteConnection($"Filename={DataSource}; Password={pass}");
        connection.Open();
        _connection = connection;
    }

    public static void CloseSqlConnection()
    {
        _connection.Close();
    }

    public void Dispose()
    {
        _connection.Close();
    }

    public static async void InitializeDatabase()
    {
        string tableCommand = """
                PRAGMA foreign_keys = off;
                BEGIN TRANSACTION;
                -- Table: Subscriptions
                CREATE TABLE IF NOT EXISTS Subscriptions (
                    DisplayName    TEXT NOT NULL CONSTRAINT UQ_DisplayName UNIQUE ON CONFLICT IGNORE,
                    SubscriptionId TEXT (200) PRIMARY KEY  UNIQUE ON CONFLICT IGNORE,
                    TenantId       TEXT (200)
                );
                CREATE UNIQUE INDEX IF NOT EXISTS IX_Subscriptions_DisplayName_SubscriptionsId ON Subscriptions (
                    SubscriptionId ASC,
                    DisplayName ASC
                );
                -- Table: QuickAccess
                CREATE TABLE IF NOT EXISTS QuickAccess (
                    Id                      INTEGER NOT NULL CONSTRAINT PK_QuickAccess PRIMARY KEY AUTOINCREMENT,
                    Name                    TEXT    NOT NULL,
                    VaultUri                TEXT    NOT NULL,
                    KeyVaultId              TEXT    NOT NULL CONSTRAINT UQ_KeyVaultId UNIQUE ON CONFLICT IGNORE,
                    SubscriptionDisplayName TEXT,
                    SubscriptionId          TEXT,
                    TenantId                TEXT    NOT NULL,
                    Location                TEXT    NOT NULL
                );
                -- Index: IX_QuickAccess_KeyVaultId
                CREATE INDEX IF NOT EXISTS IX_QuickAccess_KeyVaultId ON QuickAccess (
                    KeyVaultId
                );
                COMMIT TRANSACTION;
                PRAGMA foreign_keys = on;
                """;
        var createTableCommand = _connection.CreateCommand();
        createTableCommand.CommandText = tableCommand;
        await createTableCommand.ExecuteNonQueryAsync();
    }

    public IEnumerable<QuickAccess> GetQuickAccessItems()
    {
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, VaultUri, KeyVaultId, SubscriptionDisplayName, SubscriptionId, TenantId, Location FROM QuickAccess;";

        var reader = command.ExecuteReader();

        var items = new List<QuickAccess>();
        while (reader.Read())
        {
            var item = new QuickAccess
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                VaultUri = reader.GetString(2),
                KeyVaultId = reader.GetString(3),
                SubscriptionDisplayName = reader.IsDBNull(4) ? null : reader.GetString(4),
                SubscriptionId = reader.IsDBNull(5) ? null : reader.GetString(5),
                TenantId = reader.GetString(6),
                Location = reader.GetString(7),
            };
            items.Add(item);
        }
        return items;
    }

    public async IAsyncEnumerable<QuickAccess> GetQuickAccessItemsAsyncEnumerable()
    {
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, VaultUri, KeyVaultId, SubscriptionDisplayName, SubscriptionId, TenantId, Location FROM QuickAccess;";

        var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var item = new QuickAccess
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                VaultUri = reader.GetString(2),
                KeyVaultId = reader.GetString(3),
                SubscriptionDisplayName = reader.IsDBNull(4) ? null : reader.GetString(4),
                SubscriptionId = reader.IsDBNull(5) ? null : reader.GetString(5),
                TenantId = reader.GetString(6),
                Location = reader.GetString(7),
            };
            yield return item;
        }
    }

    public async Task<bool> QuickAccessItemByKeyVaultIdExists(string keyVaultId)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM QuickAccess WHERE KeyVaultId = @KeyVaultId LIMIT 1;";
        command.Parameters.Add(new SqliteParameter("@KeyVaultId", keyVaultId));

        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }

    public async Task<bool> DeleteQuickAccessItemByKeyVaultId(string keyVaultId)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "DELETE FROM QuickAccess WHERE KeyVaultId = @KeyVaultId;";
        command.Parameters.Add(new SqliteParameter("@KeyVaultId", keyVaultId));

        var rowsAffected = await command.ExecuteNonQueryAsync();

        // Check if any rows were deleted (1 or more indicates success)
        return rowsAffected > 0;
    }

    public async Task InsertQuickAccessItemAsync(QuickAccess item)
    {
        var command = _connection.CreateCommand();
        command.CommandText = """
                            INSERT INTO QuickAccess (Name, VaultUri, KeyVaultId, SubscriptionDisplayName, SubscriptionId, TenantId, Location)
                            VALUES (@Name, @VaultUri, @KeyVaultId, @SubscriptionDisplayName, @SubscriptionId, @TenantId, @Location);
                            """;
        command.Parameters.Add(new SqliteParameter("@Name", item.Name));
        command.Parameters.Add(new SqliteParameter("@VaultUri", item.VaultUri));
        command.Parameters.Add(new SqliteParameter("@KeyVaultId", item.KeyVaultId));
        command.Parameters.Add(new SqliteParameter("@SubscriptionDisplayName", item.SubscriptionDisplayName ?? (object)DBNull.Value));
        command.Parameters.Add(new SqliteParameter("@SubscriptionId", item.SubscriptionId ?? (object)DBNull.Value));
        command.Parameters.Add(new SqliteParameter("@TenantId", item.TenantId));
        command.Parameters.Add(new SqliteParameter("@Location", item.Location));
        await command.ExecuteNonQueryAsync();
    }

    public async Task<AppSettings> GetToggleSettings()
    {
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT Name, Value FROM SETTINGS";
        var settings = new AppSettings();
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            Enum.TryParse(reader.GetString(0), true, out SettingType parsedEnumValue);
            switch (parsedEnumValue)
            {
                case SettingType.BackgroundTransparency:
                    settings.BackgroundTransparency = reader.GetBoolean(1);
                    break;

                case SettingType.ClipboardTimeout:
                    settings.ClipboardTimeout = reader.GetInt32(1);
                    break;

                default:
                    break;
            }
        }
        return settings;
    }

    public async Task<IEnumerable<Subscriptions>> GetStoredSubscriptions()
    {
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT DisplayName, SubscriptionId, TenantId FROM Subscriptions;";

        var reader = command.ExecuteReader();

        var subscriptions = new List<Subscriptions>();
        while (await reader.ReadAsync())
        {
            var subscription = new Subscriptions
            {
                DisplayName = reader.GetString(0),
                SubscriptionId = reader.GetString(1),
                TenantId = reader.GetGuid(2)
            };
            subscriptions.Add(subscription);
        }
        return subscriptions;
    }

    public async Task InsertSubscriptions(IEnumerable<Subscriptions> subscriptions)
    {
        foreach (var subscription in subscriptions)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT OR IGNORE INTO Subscriptions (DisplayName, SubscriptionId, TenantId) VALUES (@DisplayName, @SubscriptionId, @TenantId);";
            command.Parameters.Add(new SqliteParameter("@DisplayName", subscription.DisplayName));
            command.Parameters.Add(new SqliteParameter("@SubscriptionId", subscription.SubscriptionId));
            command.Parameters.Add(new SqliteParameter("@TenantId", subscription.TenantId));
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task RemoveSubscriptionsBySubscriptionIDs(IEnumerable<string> subscriptionIds)
    {
        var command = _connection.CreateCommand();
        var paramString = new StringBuilder("DELETE FROM Subscriptions WHERE SubscriptionId IN (");

        subscriptionIds.TryGetNonEnumeratedCount(out int count);
        foreach (var (subscriptionId, index) in subscriptionIds.Select((id, index) => (id, index)))
        {
            command.Parameters.Add(new SqliteParameter("@SubscriptionId" + index, subscriptionId));
            paramString.Append(index > 0 && index > count ? ',' : "");
            paramString.Append($"@SubscriptionId{index}");
        }
        paramString.Append(')');

        command.CommandText = paramString.ToString();

        await command.ExecuteNonQueryAsync();
    }
}