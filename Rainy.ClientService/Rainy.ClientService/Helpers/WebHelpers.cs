using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Rainy.ClientService.Helpers
{
    public class WebHelpers
    {
        //private static string GetMD5FromFile(string fileName)
        //{
        //    try
        //    {
        //        FileStream file = new FileStream(fileName, FileMode.Open);
        //        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        //        byte[] retVal = md5.ComputeHash(file);
        //        file.Close();

        //        StringBuilder sb = new StringBuilder();
        //        for (int i = 0; i < retVal.Length; i++)
        //        {
        //            sb.Append(retVal[i].ToString("x2"));
        //        }
        //        return sb.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("GetMD5HashFromFile() fail, error:" + ex.Message);
        //    }
        //}

        //public static UpgradeVersions GetAllFilesInfo(string directoryPath)
        //{
        //    DirectoryInfo clientDirect = new DirectoryInfo(directoryPath);

        //    var files = clientDirect.GetFiles().ToList();


        //    //读取JSON配置文件
        //    var jsonFile = new FileInfo(HttpContext.Current.Server.MapPath("~/ClientUpgrade/UpgradeConfig.json"));

        //    var jsonStr = jsonFile.OpenText().ReadToEnd();

        //    var fileVersions = JsonConvert.DeserializeObject<UpgradeVersions>(jsonStr);
        //    var filesObj = fileVersions.Versions.Where(m => m.IsLastVersion).SingleOrDefault();

        //    ////转换为大写后分割
        //    //var ignoreExtensions = filesObj.ExtensionToIgnore;
        //    //var ingoreFiles = filesObj.FileToIgnore;

        //    ////过滤需要忽略的文件和后缀名
        //    //files = files.Where(m => filesObj.FilesToUpgrade.Exists(fileName => fileName.ToUpper().Contains(m.Name.ToUpper())) && !ingoreFiles.Contains(m.Name.ToUpper()) && !ignoreExtensions.Contains(m.Extension.ToUpper())).ToList();

        //    //filesObj.FileNamesMD5 = new Dictionary<string, string>();

        //    //files.ForEach(m => filesObj.FileNamesMD5.Add(m.FullName.Remove(0, directoryPath.Length + 1), GetMD5FromFile(m.FullName)));

        //    return fileVersions;
        //}


    }
}