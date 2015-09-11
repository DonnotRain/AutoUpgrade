 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rainy.UpgradeTool
{
    public static class UpgradeHelper
    {
        public static async Task<bool> CheckUpdate()
        {
            var currentVersion = UpgradeSettings.Instance["CurrentVersion"].ToString();

            var currentUpdateTime = DateTime.Parse(UpgradeSettings.Instance["LastUpdateTime"].ToString());

            var client = UpgradeSettings.Instance.GetUpgradeHttpClient();

            try
            {
                var response = await client.GetAsync("api/ClientVersion/LastestVersion");

                var clientVersion = await response.Content.ReadAsAsync<UpgradeFileInfo>();

                var lastUpdateTime = DateTime.Parse(clientVersion.LastUpdateTime);

                if (currentUpdateTime < lastUpdateTime && currentVersion != clientVersion.VersionName)
                    return true;
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {

            }
            catch (HttpRequestException ex)
            {

            }
            catch (Exception ex)
            {

            }

            return false;

        }


        public static async Task<UpgradeFileInfo> TryGetNewVersion()
        {
            var currentVersion = UpgradeSettings.Instance["CurrentVersion"].ToString();

            var currentUpdateTime = DateTime.Parse(UpgradeSettings.Instance["LastUpdateTime"].ToString());

            var client = UpgradeSettings.Instance.GetUpgradeHttpClient();

            try
            {
                var response = await client.GetAsync("api/ClientVersion/LastestVersion");

                var clientVersion = await response.Content.ReadAsAsync<UpgradeFileInfo>();

                var lastUpdateTime = DateTime.Parse(clientVersion.LastUpdateTime);

                if (currentUpdateTime < lastUpdateTime && currentVersion != clientVersion.VersionName)
                    return clientVersion;
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {

            }
            catch (HttpRequestException ex)
            {

            }
            catch (Exception ex)
            {

            }

            return null;
        }

    }
}
