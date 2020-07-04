using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PaperX_SCG_forms
{
    public static class Constants
    {
        public const string DatabaseFilename = "dbPaperX.db3";
        public const string DatabaseFilenameLog = "dbPaperXLog.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
              //  var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
             var   basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PaperX");
                if (!System.IO.Directory.Exists(basePath))
                {
                    System.IO.Directory.CreateDirectory(basePath);
                }
                return Path.Combine(basePath, DatabaseFilename);
            }
        }
        public static string DatabasePathLog
        {
            get
            {
                // var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
              var   basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PaperX");
                if (!System.IO.Directory.Exists(basePath))
                {
                    System.IO.Directory.CreateDirectory(basePath);
                }
                return Path.Combine(basePath, DatabaseFilenameLog);
            }
        }
    }
}
