using Rainy.UpgradeTool;
using System;
using System.Diagnostics;
using System.Windows;
using System.IO;

namespace Rainy.SampleApplication
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CheckUpgrate();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void CheckUpgrate()
        {

            btnLogin.IsEnabled = false;

            btnLogin.Content = "检查更新中...";

            var checkResult = await UpgradeHelper.CheckUpdate();

            if (checkResult)
            {
                //启动更新程序
                Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpgradeTools/Rainy.UpgradeTool.exe"));

                //当前运行WPF程序的进程实例
                Process process = Process.GetCurrentProcess();
                //遍历WPF程序的同名进程组
                foreach (Process p in Process.GetProcessesByName(process.ProcessName))
                {
                    //关闭全部进程
                    p.Kill();//这个地方用kill 而不用Shutdown();
                    //Application.Current.Shutdown();
                    return;
                }
            }

            btnLogin.IsEnabled = true;
            btnLogin.Content = "登录";
        }

    }
}
