using kvexplorer.shared.Models;
using Microsoft.Data.Sqlite;

namespace kvexplorer.shared.Database;

public partial class KvExplorerDb
{
    public KvExplorerDb()
    {
    }

    private static SqliteConnection NewSqlConnection()
    {
        string DataSource = Path.Combine(Constants.LocalAppDataFolder, "kvexplorer.db");
        using var connection = new SqliteConnection($"Filename={DataSource}");
        return connection;
    }

    public static async void InitializeDatabase()
    {
        var connection = NewSqlConnection();
        await connection.OpenAsync();
        string tableCommand = """
                PRAGMA foreign_keys = off;
                BEGIN TRANSACTION;
                -- Table: BookmarkedItems
                CREATE TABLE IF NOT EXISTS BookmarkedItems (
                    Id                      INTEGER NOT NULL
                                                    CONSTRAINT PK_BookmarkedItems PRIMARY KEY AUTOINCREMENT,
                    Name                    TEXT    NOT NULL,
                    VaultUri                TEXT    NOT NULL,
                    Type                    INT     NOT NULL,
                    SubscriptionDisplayName TEXT,
                    ContentType             TEXT,
                    Version                 TEXT    NOT NULL
                );
                -- Table: Subscriptions
                CREATE TABLE Subscriptions (
                    DisplayName    TEXT,
                    SubscriptionId TEXT (200) PRIMARY KEY,
                    TenantId       TEXT (200),
                    CONSTRAINT UQ_SubscriptionId UNIQUE (
                        SubscriptionId ASC
                    )
                    ON CONFLICT IGNORE
                );
                CREATE UNIQUE INDEX IX_Subscriptions_DisplayName_SubscriptionsId ON Subscriptions (
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
                -- Index: IX_BookmarkedItems_Name_Version
                CREATE UNIQUE INDEX IF NOT EXISTS IX_BookmarkedItems_Name_Version ON BookmarkedItems (
                    "Name",
                    "Version"
                );
                -- Index: IX_QuickAccess_KeyVaultId
                CREATE INDEX IF NOT EXISTS IX_QuickAccess_KeyVaultId ON QuickAccess (
                    KeyVaultId
                );
                COMMIT TRANSACTION;
                PRAGMA foreign_keys = on;

                -- Table: Settings
                DROP TABLE IF EXISTS Settings;
                CREATE TABLE IF NOT EXISTS Settings ( Name  TEXT (200)  PRIMARY KEY UNIQUE, Value INTEGER (1) CONSTRAINT DEFAULT_FALSE DEFAULT (0) );
                INSERT OR IGNORE INTO Settings ( Name, Value )
                VALUES ( 'BackgroundTransparency', 0 ), ( 'ClipboardTimeout', 20 );
                """;

        var createTableCommand = connection.CreateCommand();
        createTableCommand.CommandText = tableCommand;
        await createTableCommand.ExecuteNonQueryAsync();
    }

    public IEnumerable<QuickAccess> GetQuickAccessItems()
    {
        var connection = NewSqlConnection();
        connection.Open();
        var command = connection.CreateCommand();
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
        var connection = NewSqlConnection();
        await connection.OpenAsync();
        var command = connection.CreateCommand();
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
        var connection = NewSqlConnection();
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM QuickAccess WHERE KeyVaultId = @KeyVaultId LIMIT 1;";
        command.Parameters.Add(new SqliteParameter("@KeyVaultId", keyVaultId));

        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }

    public async Task<bool> DeleteQuickAccessItemByKeyVaultId(string keyVaultId)
    {
        var connection = NewSqlConnection();
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM QuickAccess WHERE KeyVaultId = @KeyVaultId;";
        command.Parameters.Add(new SqliteParameter("@KeyVaultId", keyVaultId));

        var rowsAffected = await command.ExecuteNonQueryAsync();

        // Check if any rows were deleted (1 or more indicates success)
        return rowsAffected > 0;
    }

    public async Task InsertQuickAccessItemAsync(QuickAccess item)
    {
        var connection = NewSqlConnection();
        await connection.OpenAsync();
        var command = connection.CreateCommand();
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

    public async Task<bool> UpdateToggleSettings(SettingType name, bool value)
    {
        var connection = NewSqlConnection();
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE SETTINGS SET Value = @SettingValue WHERE Name = @Name;";
        command.Parameters.Add(new SqliteParameter("@SettingValue", value ? 1 : 0));
        command.Parameters.Add(new SqliteParameter("@Name", name.ToString()));
        var rowsAffected = await command.ExecuteNonQueryAsync();
        // Check if any rows were deleted (1 or more indicates success)
        return rowsAffected > 0;
    }

    public async Task<T> UpdateToggleSettings<T>(SettingType name, T value)
    {
        var connection = NewSqlConnection();
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE SETTINGS SET Value = @SettingValue WHERE Name = @Name;";
        command.Parameters.Add(new SqliteParameter("@SettingValue", value));
        command.Parameters.Add(new SqliteParameter("@Name", name.ToString()));
        var rowsAffected = await command.ExecuteNonQueryAsync();
        // Check if any rows were deleted (1 or more indicates success)
        return rowsAffected > 0 ? value : default;
    }

    public async Task<AppSettings> GetToggleSettings()
    {
        var connection = NewSqlConnection();
        await connection.OpenAsync();
        var command = connection.CreateCommand();
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

    public static IEnumerable<Subscriptions> GetAllSubscriptions()
    {
        var connection = NewSqlConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT DisplayName, SubscriptionId, TenantId FROM Subscriptions;";

        var reader = command.ExecuteReader();

        var subscriptions = new List<Subscriptions>();
        while (reader.Read())
        {
            var subscription = new Subscriptions
            {
                DisplayName = reader.GetString(0),
                SubscriptionId = reader.GetString(1),
                TenantId = reader.GetString(2)
            };
            subscriptions.Add(subscription);
        }
        return subscriptions;
    }

    public static async Task InsertSubscription(Subscriptions subscription)
    {
        var connection = NewSqlConnection();
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Subscriptions (DisplayName, SubscriptionId, TenantId) VALUES (@DisplayName, @SubscriptionId, @TenantId);";
        command.Parameters.AddWithValue("@DisplayName", subscription.DisplayName);
        command.Parameters.AddWithValue("@SubscriptionId", subscription.SubscriptionId);
        command.Parameters.AddWithValue("@TenantId", subscription.TenantId);
        await command.ExecuteNonQueryAsync();
    }

    public IEnumerable<Subscriptions> QuerySubscriptions(Func<Subscriptions, bool> predicate)
    {
        return GetAllSubscriptions().Where(predicate);
    }
}