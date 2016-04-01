using System.IO;
using Windows.Storage;

namespace MAD_Todo.Utils {
    /// <summary>
    /// Constants and other resources used in this app.
    /// </summary>
    public class C {
        // SQLite
        public static string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db_todo.sqlite");
        public static string DB_TABLE_TODO = "todo";
        // DateTime format
        public static string YMD_FORMAT = "yyyyMMdd";
    }
}
