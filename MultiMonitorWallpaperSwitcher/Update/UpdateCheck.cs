using MultiMonitorWallpaperSwitcher.Profile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitorWallpaperSwitcher.Update
{
    public class UpdateCheck
    {
        #region 事件
        public delegate void hasUp(object sender, bool isError, string version);
        public event hasUp? HasUpdated;

        protected virtual void OnHasUpdated(bool isE, string ver)
        {
            HasUpdated?.Invoke(this, isE, ver);
        }
        #endregion

        #region 属性
        private string versionValue;
        private bool isError = false;
        #endregion

        public UpdateCheck()
        {
            isError = false;
            versionValue = UserProfile.GetLatestVersion();
        }

        public async void UpdateChecking()
        {
            isError = false;
            try
            {
                Uri uri = new Uri("https://api.github.com/repos/LightAPIs/MultiMonitorWallpaperSwitcher/releases/latest");
                HttpClient request = new HttpClient();
                ProductHeaderValue header = new ProductHeaderValue("MultiMonitorWallpaperSwitcher", "1.0.0");
                ProductInfoHeaderValue infoHeader = new ProductInfoHeaderValue(header);
                request.DefaultRequestHeaders.UserAgent.Add(infoHeader);
                request.Timeout = TimeSpan.FromSeconds(5);
                HttpResponseMessage response = await request.GetAsync(uri);
                string responseText = await response.Content.ReadAsStringAsync();
                LatestInfo? info = JsonConvert.DeserializeObject<LatestInfo>(responseText.Trim());
                if (info != null && !string.IsNullOrEmpty(info.tag_name))
                {
                    string readVersion = info.tag_name.Replace("v", "");
                    if (!string.IsNullOrEmpty(readVersion) && versionValue != readVersion)
                    {
                        versionValue = readVersion;
                        UserProfile.SetLatestVersion(readVersion);
                    }
                }
                else
                {
                    isError = true;
                }
            }
            catch (Exception)
            {
                isError = false;
            }
            finally
            {
                OnHasUpdated(isError, versionValue);
            }
        }
    }

    public class LatestInfo
    {
        public string? url { get; set; }
        public string? html_url { get; set; }
        public string? id { get; set; }
        public string? tag_name { get; set; }
        public string? name { get; set; }
        public string? created_at { get; set; }
        public string? published_at { get; set; }
    }
}
