//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Data.Sqlite;
//using System.Collections.Generic;
//using Microsoft.Identity.Client;
//using Microsoft.EntityFrameworkCore;

//namespace kvexplorer.shared.Database;

//public partial class KvExplorerDbContextEx1 : KvExplorerDbContext
//{

//    public KvExplorerDbContextEx1(DbContextOptions options) : base(options)
//    {
//    }


//    public async static void InitializeDatabase()
//    {
//        //var x = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\offline.db";

//        //var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
//        //var folder = await storage.TryGetFolderFromPathAsync(new Uri(Constants.LocalAppDataFolder));
//        //var dbpath = await storage.TryGetFileFromPathAsync(new Uri(Constants.LocalAppDataFolder + "kvexplorer.db"));

//        //await ApplicationData.Current.LocalFolder .CreateFileAsync("sqliteSample.db", CreationCollisionOption.OpenIfExists); 
//        string dbpath = Path.Combine(Constants.LocalAppDataFolder, "kvexplorer.db");
//        using (var db = new SqliteConnection($"Filename={dbpath}"))
//        {
//            db.Open();

//            string tableCommand = "CREATE TABLE IF NOT " +
//                "EXISTS Bookmark (Primary_Key INTEGER PRIMARY KEY, " +
//                "Text_Entry NVARCHAR(2048) NULL)";

//            var createTable = new SqliteCommand(tableCommand, db);

//            createTable.ExecuteReader();
//        }
//    }
//    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code.
//    //You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148.
//    //For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.


//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        optionsBuilder.EnableSensitiveDataLogging();
//        base.OnConfiguring(optionsBuilder);
//    }

//    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    //    => optionsBuilder.UseSqlite("Data Source=C:\\repos\\sidestep\\kvexplorer.db");

//}
