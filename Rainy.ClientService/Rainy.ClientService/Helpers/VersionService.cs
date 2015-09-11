using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Rainy.ClientService.Helpers
{
    public static class VersionService
    {
        private static string configSavePath = "~/ClientUpgrade/UpgradeConfig.json";

        public static UpgradeVersions GetLastestVersion()
        {
            //读取JSON配置文件
            var jsonFile = new FileInfo(HttpContext.Current.Server.MapPath(configSavePath));
            var fileReader = jsonFile.OpenText();
            var jsonStr = fileReader.ReadToEnd();
            fileReader.Close();

            var fileVersions = JsonConvert.DeserializeObject<UpgradeVersions>(jsonStr);
            return fileVersions;
        }

        public static void SaveVersionInfo(UpgradeVersions versionInfo)
        {
            //读取JSON配置文件
            var jsonFile = new FileInfo(HttpContext.Current.Server.MapPath(configSavePath));

            jsonFile.Delete();

            var jsonStr = JsonConvert.SerializeObject(versionInfo);
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonStr);


            var fileReader = jsonFile.OpenWrite();
            fileReader.Write(jsonBytes, 0, jsonBytes.Length);

            fileReader.Flush();
            fileReader.Close();
        }

    }
}