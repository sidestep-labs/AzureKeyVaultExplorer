using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

using System.Collections.Generic;

using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Reflection.PortableExecutable;
using static Azure.Core.HttpHeader;

namespace kvexplorer.shared.Database;

public partial class KvExplorerDb
{
    public KvExplorerDb()
    {
    }

    public static async Task<SqliteConnection> OpenSqlConnection()
    {
        string DataSource = Path.Combine(Constants.LocalAppDataFolder, "kvexplorer.db");
        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = DataSource;
        using var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
        //using var db = new SqliteConnection($"Filename={DataSource}");
        await connection.OpenAsync();
        return connection;
    }

    public static async void InitializeDatabase()
    {
        //var x = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\offline.db";
        //var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        //var folder = await storage.TryGetFolderFromPathAsync(new Uri(Constants.LocalAppDataFolder));
        //var dbpath = await storage.TryGetFileFromPathAsync(new Uri(Constants.LocalAppDataFolder + "kvexplorer.db"));
        //await ApplicationData.Current.LocalFolder .CreateFileAsync("sqliteSample.db", CreationCollisionOption.OpenIfExists);

        //using var db = new SqliteConnection($"Filename={DataSource}");
        //db.Open();

        var connection = await OpenSqlConnection();
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
                -- Table: QuickAccess
                CREATE TABLE IF NOT EXISTS QuickAccess (
                    Id                      INTEGER NOT NULL
                                                    CONSTRAINT PK_QuickAccess PRIMARY KEY AUTOINCREMENT,
                    Name                    TEXT    NOT NULL,
                    VaultUri                TEXT    NOT NULL,
                    KeyVaultId              TEXT    NOT NULL,
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
                COMMIT TRANSACTION;
                PRAGMA foreign_keys = on;
                """;

        var createTableCommand = connection.CreateCommand();
        createTableCommand.CommandText = tableCommand;
        await createTableCommand.ExecuteNonQueryAsync();
    }

    //public async Task<IEnumerable<QuickAccess>> GetQuickAccessItemsAsync()
    //{
    //    var connection = await OpenSqlConnection();
    //    var command = connection.CreateCommand();
    //    command.CommandText = "SELECT Id, Name, VaultUri, KeyVaultId, SubscriptionDisplayName, SubscriptionId, TenantId, Location FROM QuickAccess;";

    //    var reader = await command.ExecuteReaderAsync();

    //    var items = new List<QuickAccess>();
    //    while (reader.Read())
    //    {
    //        var item = new QuickAccess
    //        {
    //            Id = reader.GetInt32(0),
    //            Name = reader.GetString(1),
    //            VaultUri = reader.GetString(2),
    //            KeyVaultId = reader.GetString(3),
    //            SubscriptionDisplayName = reader.GetString(4) ?? null,
    //            SubscriptionId = reader.GetString(5) ?? null,
    //            TenantId = reader.GetString(6),
    //            Location = reader.GetString(7),
    //        };
    //        items.Add(item);
    //    }
    //    return items;
    //}


    public async IAsyncEnumerable<QuickAccess> GetQuickAccessItemsAsync()
    {
        var connection = await OpenSqlConnection();
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
                SubscriptionDisplayName = reader.GetString(4) ?? null,
                SubscriptionId = reader.GetString(5) ?? null,
                TenantId = reader.GetString(6),
                Location = reader.GetString(7),
            };
            yield return item;
        }
    }


    public async Task<bool> QuickAccessItemByKeyVaultIdExists(string keyVaultId)
    {
        var connection = await OpenSqlConnection();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM QuickAccess WHERE KeyVaultId = @KeyVaultId LIMIT 1;";
        command.Parameters.Add(new SqliteParameter("@KeyVaultId", keyVaultId));

        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }


    public async Task<bool> DeleteQuickAccessItemByKeyVaultId(string keyVaultId)
    {
        var connection = await OpenSqlConnection();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM QuickAccess WHERE KeyVaultId = @KeyVaultId;";
        command.Parameters.Add(new SqliteParameter("@KeyVaultId", keyVaultId));

        var rowsAffected = await command.ExecuteNonQueryAsync();

        // Check if any rows were deleted (1 or more indicates success)
        return rowsAffected > 0;
    }


}