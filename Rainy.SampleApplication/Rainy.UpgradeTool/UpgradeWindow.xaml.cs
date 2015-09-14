
using Rainy.UpgradeTool;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
            tbkCurrentVersion.Text = "当前客户端版本:" + UpgradeSettings.Instance["CurrentVersion"];

            this.MouseDown += MainWindow_MouseDown;

            tbkUpgradeInfo.Text = "检查更新中...";

            _lastestVersionInfo = await UpgradeHelper.TryGetNewVersion();

            //有更新则直接更新掉
            if (_lastestVersionInfo != null) ExcuteUpgrade();
            else FinishUpgrade();
        }

        private async void ExcuteUpgrade()
        {
            prgUpgrade.Maximum = _lastestVersionInfo.FilesSize;
            prgUpgrade.Value = 0;
            tbkVersionInfo.Text = _lastestVersionInfo.Description;
            tbkTragetVersion.Text = "升级到最新版本：" + _lastestVersionInfo.VersionName;

            //删除需要删除的数据
            if (_lastestVersionInfo.FileToDelete != null)
                foreach (var fileName in _lastestVersionInfo.FileToDelete)
                {
                    var file = new FileInfo(System.IO.Path.Combine(UpgradeSettings.GetBaseDirectory(), fileName));

                    if (file.Exists) file.Delete();
                }

            var client = UpgradeSettings.Instance.GetUpgradeHttpClient();

            foreach (var fileName in _lastestVersionInfo.FilesToUpgrade)
            {
                var currentIndex = _lastestVersionInfo.FilesToUpgrade.IndexOf(fileName) + 1;

                tbkUpgradeInfo.Text = "更新中，进度 第" + currentIndex.ToString() + "个文件 / 共" + _lastestVersionInfo.FilesToUpgrade.Count.ToString() + "个文件 ( " + Math.Round(prgUpgrade.Value / prgUpgrade.Maximum * 100) + "% )";

                tbkFileUpgrading.Text = fileName;

                var response = await client.GetAsync("api/ClientVersion/File?fileName=" + fileName);

                var fileStream = await response.Content.ReadAsStreamAsync();

                var file = new FileInfo(System.IO.Path.Combine(UpgradeSettings.GetBaseDirectory(), fileName));
                if (!file.Directory.Exists)
                    file.Directory.Create();

                var stream = file.OpenWrite();

                await fileStream.CopyToAsync(stream);

                prgUpgrade.Value += stream.Length / 1024;

                stream.Close();

                if (currentIndex == _lastestVersionInfo.FilesToUpgrade.Count)
                {

                    if (_lastestVersionInfo != null)
                    {
                        //更新配置文件
                        UpgradeSettings.Instance["CurrentVersion"] = _lastestVersionInfo.VersionName;
                        UpgradeSettings.Instance["LastUpdateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    tbkFileUpgrading.Text = "无"; 
                    tbkUpgradeInfo.Text = "更新完成，进度 第" + currentIndex.ToString() + "个文件 / 共" + _lastestVersionInfo.FilesToUpgrade.Count.ToString() + "个文件 ( " + Math.Round(prgUpgrade.Value / prgUpgrade.Maximum * 100) + "% )";
                    tbkUpgradeInfo.Foreground = new SolidColorBrush(Colors.White);

                    textBlock.Visibility = Visibility.Visible;
                    button.Visibility = Visibility.Visible;
                }
             }
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            FinishUpgrade();
        }
    }
}
