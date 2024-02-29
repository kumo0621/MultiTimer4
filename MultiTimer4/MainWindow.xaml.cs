using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WMPLib; // Windows Media Playerのライブラリを使用するための参照

namespace MultiTimer4
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer1, timer2, timer3, timer4;
        private int timeLeft1, timeLeft2, timeLeft3, timeLeft4;
        private WindowsMediaPlayer player = new WindowsMediaPlayer(); // MediaPlayerのインスタンス
        private string soundsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");

        public MainWindow()
        {
            InitializeComponent();
            EnsureSoundsDirectoryAndFilesExist();
        }

        private void EnsureSoundsDirectoryAndFilesExist()
        {
            if (!Directory.Exists(soundsDirectory))
            {
                Directory.CreateDirectory(soundsDirectory);
            }

            // 4つのタイマー用のダミーMP3ファイルを確認し、存在しない場合は作成
            for (int i = 1; i <= 4; i++)
            {
                string filePath = Path.Combine(soundsDirectory, $"timer{i}.mp3");
                if (!File.Exists(filePath))
                {
                    // ファイルが存在しない場合、ダミーファイルを作成
                    // 実際のアプリケーションでは、事前に用意したMP3ファイルをコピーするなどの方法に変更してください
                    File.Create(filePath).Dispose();
                }
            }
        }

        private void StartButton1_Click(object sender, RoutedEventArgs e) => StartTimer(timeTextBox1, ref timer1, 1, statusTextBlock1);
        private void StartButton2_Click(object sender, RoutedEventArgs e) => StartTimer(timeTextBox2, ref timer2, 2, statusTextBlock2);
        private void StartButton3_Click(object sender, RoutedEventArgs e) => StartTimer(timeTextBox3, ref timer3, 3, statusTextBlock3);
        private void StartButton4_Click(object sender, RoutedEventArgs e) => StartTimer(timeTextBox4, ref timer4, 4, statusTextBlock4);

        private void StartTimer(TextBox timeTextBox, ref DispatcherTimer timer, int timerNumber, TextBlock statusTextBlock)
        {
            if (int.TryParse(timeTextBox.Text, out var time) && time > 0)
            {
                switch (timerNumber)
                {
                    case 1: timeLeft1 = time; break;
                    case 2: timeLeft2 = time; break;
                    case 3: timeLeft3 = time; break;
                    case 4: timeLeft4 = time; break;
                }

                timer?.Stop(); // 既存のタイマーを停止
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += (s, e) => Timer_Tick(s, e, timerNumber, statusTextBlock);
                timer.Start();
                statusTextBlock.Text = $"残り時間: {time}秒";
            }
            else
            {
                MessageBox.Show("有効な秒数を入力してください。");
            }
        }

        private void Timer_Tick(object sender, EventArgs e, int timerNumber, TextBlock statusTextBlock)
        {
            int timeLeft = timerNumber switch
            {
                1 => --timeLeft1,
                2 => --timeLeft2,
                3 => --timeLeft3,
                4 => --timeLeft4,
                _ => 0,
            };

            if (timeLeft > 0)
            {
                statusTextBlock.Text = $"残り時間: {timeLeft}秒";
            }
            else
            {
                (sender as DispatcherTimer)?.Stop();
                statusTextBlock.Text = "時間切れ！";
                PlaySound(timerNumber);
            }
        }

        private void PlaySound(int timerNumber)
        {
            string soundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", $"timer{timerNumber}.mp3");
            if (File.Exists(soundFilePath))
            {
                player.URL = soundFilePath;
                player.controls.play();
            }
            else
            {
                MessageBox.Show($"{soundFilePath} が見つかりません。", "エラー");
            }
        }
    }
}

