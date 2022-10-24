using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MultiMonitorWallpaperSwitcher.Config
{
    public class Database
    {
        protected string dbName;
        protected string dbPath;
        protected SQLiteConnection cn;
        protected SQLiteCommand cmd;

        public Database(string _dbName) {
            dbName = string.IsNullOrEmpty(_dbName) ? "unknown" : _dbName;

            if (!Directory.Exists(Glob.RootName)) {
                Directory.CreateDirectory(Glob.RootName);
            }

            dbPath = Path.Combine(Glob.RootName, dbName + ".db");
            cn = new SQLiteConnection("data source=" + dbPath);
            cn.Open();
            cmd = new SQLiteCommand()
            {
                Connection = cn
            };
        }

        public void CloseDatabase()
        {
            if (cn.State == System.Data.ConnectionState.Open)
            {
                cn.Close();
            }
        }

        /// <summary>
        /// 整理数据库
        /// </summary>
        public void CleanDisk()
        {
            cmd.CommandText = "VACUUM";
            cmd.ExecuteNonQuery();
        }

        public static string ConvertText(string text)
        {
            return text.Replace("'", "''");
        }

        public virtual void Init() { }
    }
}
