using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ClientLaunch
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            //if (e != null && e.Args.Length > 0)
            //{
            //    UpgradeWindow window = new UpgradeWindow(true);
            //    window.Show();
            //}
            //else
            //{
               
            //}
            //UpgradeWindow window = new UpgradeWindow();
            //window.Show();
            base.OnStartup(e);
        }
    }
}
