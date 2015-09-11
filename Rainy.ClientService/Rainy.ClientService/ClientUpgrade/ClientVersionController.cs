using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using Rainy.ClientService.Helpers;

namespace Rainy.ClientService.Controllers
{
    /// <summary>
    /// 由客户端远程调用
    /// </summary>
    public class ClientVersionController : ApiController
    {     /// <summary>
          /// 解压路径，保存在相应版本里
          /// </summary>
        private const string FILEENTRYPATH = "~/ClientUpgrade/Versions";

        [AllowAnonymous]
        [Route("api/ClientVersion/LastestVersion")]
        public UpgradeFileInfo GetVersionInfo()
        {
            var fileVersions = VersionService.GetLastestVersion();

            var lastVersion = fileVersions.Versions.Where(m => m.IsLastVersion).SingleOrDefault();

            return lastVersion;
        }

        [AllowAnonymous]
        [Route("api/ClientVersion/File")]
        public void GetFileByName(string fileName)
        {
            try
            {

                var Server = System.Web.HttpContext.Current.Server;

                var fileVersions = VersionService.GetLastestVersion();

                var lastVersion = fileVersions.Versions.Where(m => m.IsLastVersion).SingleOrDefault();

                //文件保存路径
                string pathForEntry = Server.MapPath(FILEENTRYPATH + "/" + lastVersion.VersionName);


                FileInfo fileInfo = new FileInfo(Path.Combine(pathForEntry, fileName));

                if (!fileInfo.Exists)
                {
                    throw new Exception("文件不存在");
                }

                FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open);
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileInfo.Name);
                HttpContext.Current.Response.AddHeader("Content-Length", bytes.Length.ToString());
                HttpContext.Current.Response.Charset = "UTF-8";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
                HttpContext.Current.Response.ContentType = CommonToolkit.GetContentType(fileInfo.Extension.Remove(0, 1));
                HttpContext.Current.Response.BinaryWrite(bytes);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
            catch (System.ArgumentException ex)
            {
                throw new Exception("文件名格式错误");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
