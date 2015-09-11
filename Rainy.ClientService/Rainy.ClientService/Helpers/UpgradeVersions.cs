using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rainy.ClientService.Helpers
{
    public class UpgradeVersions
    {
        public List<String> FilesToIgnore { get; set; }
        public List<String> FileExtensionToIgnore { get; set; }

        public List<UpgradeFileInfo> Versions { get; set; }
    }

    public class UpgradeFileInfo
    {
        public string VersionName { get; set; }
        public string Description { get; set; }
        public string LastUpdateTime { get; set; }

        /// <summary>
        /// 总文件大小，以kb为单位
        /// </summary>
        public double FilesSize { get; set; }

        /// <summary>
        /// 是否为初始版本
        /// </summary>
        public bool IsFirstVersion { get; set; }

        /// <summary>
        /// 是否为最新版本
        /// </summary>
        public bool IsLastVersion { get; set; }

        //文件名和文件MD5值
        public Dictionary<String, String> FileNamesMD5 { get; set; }

        //需要更新的文件
        public List<String> FilesToUpgrade { get; set; }

        //需要删除的文件
        public List<String> FileToDelete { get; set; }
    }
}