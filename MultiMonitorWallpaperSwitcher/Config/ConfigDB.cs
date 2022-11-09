using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MultiMonitorWallpaperSwitcher.Config
{
    public class ConfigDB : Database
    {
        public ConfigDB(string _dbName) : base(_dbName) {
            Init();
        }

        public override void Init()
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS monitor(device_id TEXT PRIMARY KEY NOT NULL, interval_time INT DEFAULT 0, folder_list TEXT DEFAULT '', cur_path TEXT DEFAULT '', last_path TEXT DEFAULT '');";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS wallpaper(id INTEGER PRIMARY KEY AUTOINCREMENT, create_time DATETIME, device_id TEXT, path TEXT, all_count INT DEFAULT 0, random_index INT DEFAULT -1);";
            cmd.ExecuteNonQuery();
        }

        #region Monitor
        public void SetMonitor(string device_id, int interval_time, string folder_list, string cur_path = "", string last_path = "")
        {
            string deviceId = ConvertText(device_id);
            cmd.CommandText = $"SELECT device_id FROM monitor WHERE device_id='{deviceId}';";
            object readNum = cmd.ExecuteScalar();
            if (readNum == null)
            {   //* Insert
                cmd.CommandText = $"INSERT INTO monitor VALUES('{deviceId}', {interval_time}, '{ConvertText(folder_list)}', '{ConvertText(cur_path)}', '{ConvertText(last_path)}');";
                cmd.ExecuteNonQuery();
            }
            else
            {   //* Update
                cmd.CommandText = $"UPDATE monitor SET interval_time={interval_time}, folder_list='{ConvertText(folder_list)}', cur_path='{ConvertText(cur_path)}', last_path='{ConvertText(last_path)}' WHERE device_id='{deviceId}';";
                cmd.ExecuteNonQuery();
            }
        }

        public ConfigDataSet.MonitorDataTable GetAllMonitor()
        {
            cmd.CommandText = $"SELECT * FROM monitor;";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            ConfigDataSet.MonitorDataTable monitor = new ConfigDataSet.MonitorDataTable();
            adapter.Fill(monitor);
            return monitor;
        }

        public ConfigDataSet.MonitorRow? GetMonitor(string device_id)
        {
            cmd.CommandText = $"SELECT * FROM monitor WHERE device_id='{ConvertText(device_id)}';";
            SQLiteDataAdapter adapater = new SQLiteDataAdapter(cmd);
            ConfigDataSet.MonitorDataTable monitors = new ConfigDataSet.MonitorDataTable();
            adapater.Fill(monitors);
            if (monitors.Count > 0)
            {
                return monitors[0];
            }
            return null;
        }

        public string GetMonitorCurPath(string device_id)
        {
            cmd.CommandText = $"SELECT cur_path FROM monitor WHERE device_id='{ConvertText(device_id)}';";
            object readValue = cmd.ExecuteScalar();
            return readValue == null ? string.Empty : readValue.ToString()!;
        }

        public string GetMonitorLastPath(string device_id)
        {
            cmd.CommandText = $"SELECT last_path FROM monitor WHERE device_id='{ConvertText(device_id)}';";
            object readValue = cmd.ExecuteScalar();
            return readValue == null ? string.Empty : readValue.ToString()!;
        }

        public void UpdateMonitorIntervalTime(string device_id, int interval_time)
        {
            cmd.CommandText = $"UPDATE monitor SET interval_time={interval_time} WHERE device_id='{ConvertText(device_id)}';";
            cmd.ExecuteNonQuery();
        }

        public void UpdateMonitorFolderList(string device_id, string folder_list)
        {
            cmd.CommandText = $"UPDATE monitor SET folder_list='{ConvertText(folder_list)}' WHERE device_id='{ConvertText(device_id)}';";
            cmd.ExecuteNonQuery();
        }

        public void UpdateMonitorWallpaperPath(string device_id, string wallpaperPath)
        {
            string lastPath = GetMonitorCurPath(device_id);
            cmd.CommandText = $"UPDATE monitor SET cur_path='{ConvertText(wallpaperPath)}', last_path='{ConvertText(lastPath)}' WHERE device_id='{ConvertText(device_id)}';";
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region Wallpaper
        public void InsertWallpaper(string create_time, string device_id, string path, int all_count = 0, int random_index = -1)
        {
            cmd.CommandText = $"INSERT INTO wallpaper VALUES(NULL, '{create_time}', '{ConvertText(device_id)}', '{ConvertText(path)}', {all_count}, {random_index});";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <returns></returns>
        public ConfigDataSet.WallpaperDataTable GetAllWallpaper()
        {
            cmd.CommandText = "SELECT * FROM wallpaper";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            ConfigDataSet.WallpaperDataTable wallpaperRows = new ConfigDataSet.WallpaperDataTable();
            adapter.Fill(wallpaperRows);
            return wallpaperRows;
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <returns></returns>
        public long GetWallpaperCount()
        {
            cmd.CommandText = "SELECT COUNT(1) FROM wallpaper;";
            object readValue = cmd.ExecuteScalar();
            if (readValue == null )
            {
                return 0;
            }
            return Convert.ToInt64(readValue);
        }

        /// <summary>
        /// 删除所有记录
        /// </summary>
        public void DeleteAllWallpaper()
        {
            cmd.CommandText = "DELETE FROM wallpaper;";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "DELETE FROM sqlite_sequence WHERE name='wallpaper';";
            cmd.ExecuteNonQuery();
            CleanDisk();
        }
        #endregion
    }
}
