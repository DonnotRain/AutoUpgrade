 
using Rainy.UpgradeTool; 
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;


namespace RainyTools.UpgradeTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpgradeWindow : Window
    {
        private UpgradeFileInfo _lastestVersionInfo = null;

        public UpgradeWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MouseDown += MainWindow_MouseDown;

            tbkUpgradeInfo.Text = "检查更新中...";

            _lastestVersionInfo = await UpgradeHelper.TryGetNewVersion();

            //有更新则直接更新掉
            if (_lastestVersionInfo != null) ExcuteUpgrade();
            else FinishUpgrade();
        }

        private async void ExcuteUpgrade()
        {
            //删除需要删除的数据
            if (_lastestVersionInfo.FileToDelete != null)
                foreach (var fileName in _lastestVersionInfo.FileToDelete)
                {
                    var file = new FileInfo(System.IO.Path.Combine(UpgradeSettings.GetBaseDirectory(), fileName));

                    if (file.Exists) file.Delete();
                }

            var client = UpgradeSettings.Instance.GetUpgradeHttpClient();

            _lastestVersionInfo.FilesToUpgrade.ForEach(async fileName =>
           {
               var currentIndex = _lastestVersionInfo.FilesToUpgrade.IndexOf(fileName) + 1;
               tbkUpgradeInfo.Text = "更新中，进度 " + currentIndex.ToString() + "/" + _lastestVersionInfo.FilesToUpgrade.Count.ToString() + " ( " + Math.Round((double)currentIndex / (double)_lastestVersionInfo.FilesToUpgrade.Count * 100) + "% )";

               tbkFileUpgrading.Text = fileName;

               var response = await client.GetAsync("api/ClientVersion/File?fileName=" + fileName);

               var fileStream = await response.Content.ReadAsStreamAsync();

               var file = new FileInfo(System.IO.Path.Combine(UpgradeSettings.GetBaseDirectory(), fileName));
               if (!file.Directory.Exists)
                   file.Directory.Create();

               var stream = file.OpenWrite();

               await fileStream.CopyToAsync(stream);
               stream.Close();

               if (currentIndex == _lastestVersionInfo.FilesToUpgrade.Count)
               {
                   tbkUpgradeInfo.Text = "更新完成";
                   tbkFileUpgrading.Text = "无";
                   prgUpgrade.Value = 100;
                   FinishUpgrade();
               }
               else
               {
                   prgUpgrade.Value = Math.Round((double)(currentIndex + 1) / (double)_lastestVersionInfo.FilesToUpgrade.Count * 100);
               }

           });
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void FinishUpgrade()
        {
            if (_lastestVersionInfo != null)
            {
                //更新配置文件
                UpgradeSettings.Instance["CurrentVersion"] = _lastestVersionInfo.VersionName;
                UpgradeSettings.Instance["LastUpdateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            Process.Start(System.IO.Path.Combine(UpgradeSettings.GetBaseDirectory(), "Rainy.SampleApplication.exe"));

            //当前运行WPF程序的进程实例
            Process process = Process.GetCurrentProcess();

            //遍历WPF程序的同名进程组
            foreach (Process p in Process.GetProcessesByName(process.ProcessName))
            {
                //关闭全部进程
                p.Kill();//这个地方用kill 而不用Shutdown();的原因是,Shutdown关闭程序在进程管理器里进程的释放有延迟不是马上关闭进程的
                //Application.Current.Shutdown();
                return;
            }
        }
    }
}
