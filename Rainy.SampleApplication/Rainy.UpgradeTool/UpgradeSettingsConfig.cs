using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Rainy.UpgradeTool
{
    /// <summary>
    /// 辅助类，存储一些服务端的配置
    /// 此配置不随版本更新
    /// </summary>
    public class UpgradeSettings
    {
        private const string CONFIGEXEPATH = "";

        public static string GetBaseDirectory()
        {

            var baseDirectopry = AppDomain.CurrentDomain.BaseDirectory;

            if (baseDirectopry.EndsWith("UpgradeTools\\"))
            {
                baseDirectopry = baseDirectopry.Remove(baseDirectopry.IndexOf("UpgradeTools\\"));
            }

            return baseDirectopry;
        }

        public HttpClient GetUpgradeHttpClient()
        {
            var serverUrl = this["UpgradeServerUrl"];

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(serverUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public string this[string index]
        {
            get
            {
                return GetConfigNodeValue(index);

            }
            set
            {
                SetValue(index, value);
            }
        }

        /// <summary>
        /// 获取配置节点值
        /// </summary>
        /// <param name="name">配置文件字段名称</param>
        private string GetConfigNodeValue(string name)
        {
            string configPath = Path.Combine(GetBaseDirectory(), "Client.config");

            XmlDocument doc = new XmlDocument();
            doc.Load(configPath);
            XmlNode node = doc.SelectSingleNode(@"//add[@key='" + name + "']");
            XmlElement ele = (XmlElement)node;

            return ele.GetAttribute("value");

        }

        /// <summary>
        /// 更新配置文件信息
        /// </summary>
        /// <param name="name">配置文件字段名称</param>
        /// <param name="Xvalue">值</param>
        private void UpdateConfig(string name, string Xvalue)
        {
            string configPath = Path.Combine(GetBaseDirectory(), "Client.config");

            XmlDocument doc = new XmlDocument();
            doc.Load(configPath);
            XmlNode node = doc.SelectSingleNode(@"//add[@key='" + name + "']");
            XmlElement ele = (XmlElement)node;
            ele.SetAttribute("value", Xvalue);
            doc.Save(configPath);
        }

        //<summary>  
        ///向.config文件的appKey结写入信息AppValue   保存设置  
        ///</summary>  
        ///<param name="AppKey">节点名</param>  
        ///<param name="AppValue">值</param>
        private void SetValue(String AppKey, String AppValue)
        {
            string configPath = Path.Combine(GetBaseDirectory(), "Client.config");

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(configPath);
            XmlNode xNode;
            XmlElement xElem1;
            XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//appSettings");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null)
                xElem1.SetAttribute("value", AppValue);
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", AppKey);
                xElem2.SetAttribute("value", AppValue);
                xNode.AppendChild(xElem2);
            }

            xDoc.Save(configPath);
        }


        private static UpgradeSettings _instance = null;

        public static UpgradeSettings Instance
        {
            get
            {
                if (_instance == null) _instance = new UpgradeSettings();
                return _instance;
            }
        }
 
    }
}
