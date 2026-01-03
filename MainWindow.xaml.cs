using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace GoodNightPC
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer countdownTimer;
        private int countdownSeconds = 0;
        private bool isRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            AddLog("程序启动成功");
        }

        private void InitializeTimer()
        {
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(HourTextBox.Text, out int hours) || hours < 0 || hours > 23)
            {
                MessageBox.Show("小时数必须在 0-23 之间！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(MinuteTextBox.Text, out int minutes) || minutes < 0 || minutes > 59)
            {
                MessageBox.Show("分钟数必须在 0-59 之间！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(SecondTextBox.Text, out int seconds) || seconds < 0 || seconds > 59)
            {
                MessageBox.Show("秒数必须在 0-59 之间！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (hours == 0 && minutes == 0 && seconds == 0)
            {
                MessageBox.Show("请设置有效的时间！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            countdownSeconds = hours * 3600 + minutes * 60 + seconds;
            isRunning = true;
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            // 立即显示初始倒计时
            UpdateCountdownDisplay();

            // 开始倒计时
            countdownTimer.Start();

            string action = GetSelectedAction();
            AddLog($"定时任务已开始：{hours}小时{minutes}分{seconds}秒后执行【{action}】");

            string timeStr = FormatTimeString(countdownSeconds);
            StatusLabel.Text = $"状态：定时器运行中 - 还有{timeStr}后执行{action}";
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            countdownTimer.Stop();
            isRunning = false;
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            StatusLabel.Text = "状态：定时器已取消";
            CountdownLabel.Text = "倒计时：--:--:--";

            AddLog("定时任务已取消");
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (countdownSeconds > 0)
            {
                countdownSeconds--;
                UpdateCountdownDisplay();

                string action = GetSelectedAction();
                string timeStr = FormatTimeString(countdownSeconds);
                StatusLabel.Text = $"状态：定时器运行中 - 还有{timeStr}后执行{action}";
            }
            else
            {
                ExecuteShutdown();
            }
        }

        private void UpdateCountdownDisplay()
        {
            int hours = countdownSeconds / 3600;
            int minutes = (countdownSeconds % 3600) / 60;
            int seconds = countdownSeconds % 60;
            CountdownLabel.Text = $"倒计时：{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        private string FormatTimeString(int totalSeconds)
        {
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            if (hours > 0)
                return $"{hours}小时{minutes}分{seconds}秒";
            else if (minutes > 0)
                return $"{minutes}分{seconds}秒";
            else
                return $"{seconds}秒";
        }

        private void ExecuteShutdown()
        {
            countdownTimer.Stop();
            isRunning = false;

            string action = GetSelectedAction();
            string command = GetShutdownCommand();

            try
            {
                AddLog($"倒计时结束，正在执行：{action}");

                var processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(processInfo);
                AddLog($"命令已执行：{command}");
            }
            catch (Exception ex)
            {
                AddLog($"执行失败：{ex.Message}");
                MessageBox.Show($"执行失败：{ex.Message}", "错误", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                ResetUI();
            }
        }

        private string GetSelectedAction()
        {
            if (ShutdownRadio.IsChecked == true) return "关机";
            if (RestartRadio.IsChecked == true) return "重启";
            if (HibernateRadio.IsChecked == true) return "休眠";
            if (ForceHibernateRadio.IsChecked == true) return "强制休眠";
            if (ForceShutdownRadio.IsChecked == true) return "强制关机";
            return "休眠";
        }

        private string GetShutdownCommand()
        {
            if (ShutdownRadio.IsChecked == true) return "shutdown /s /t 0";
            if (RestartRadio.IsChecked == true) return "shutdown /r /t 0";
            if (HibernateRadio.IsChecked == true) return "shutdown /h";
            if (ForceHibernateRadio.IsChecked == true) return "shutdown /f /h";
            if (ForceShutdownRadio.IsChecked == true) return "shutdown /s /f /t 0";
            return "shutdown /h";
        }

        private void ResetUI()
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            StatusLabel.Text = "状态：等待设置";
            CountdownLabel.Text = "倒计时：--:--:--";
        }

        private void AddLog(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            LogTextBox.AppendText($"[{timestamp}] {message}\n");
            LogTextBox.ScrollToEnd();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            
            if (isRunning)
            {
                var result = MessageBox.Show("定时器正在运行，确定要退出吗？", "确认",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}

