using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Microsoft.Identity.Client;

namespace kvexplorer.shared.Services
{
    public class KvDbContext
    {
        public async static void InitializeDatabase()
        {
            var x = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\offline.db";

            //var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
            //var folder = await storage.TryGetFolderFromPathAsync(new Uri(Constants.LocalAppDataFolder));
            //var dbpath = await storage.TryGetFileFromPathAsync(new Uri(Constants.LocalAppDataFolder + "kvexplorer.db"));

            //await ApplicationData.Current.LocalFolder .CreateFileAsync("sqliteSample.db", CreationCollisionOption.OpenIfExists); 
            string dbpath = Path.Combine(Constants.LocalAppDataFolder, "kvexplorer.db");
            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                string tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Bookmark (Primary_Key INTEGER PRIMARY KEY, " +
                    "Text_Entry NVARCHAR(2048) NULL)";

                var createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }
    }
}
