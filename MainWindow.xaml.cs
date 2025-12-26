using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Timer = System.Timers.Timer;

namespace VpnWidget
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient client = new HttpClient();
        private readonly Timer refreshTimer = new Timer(30000);
        private bool isDarkTheme = true;

        public MainWindow()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            client = new HttpClient(handler);
            InitializeComponent();
            AddToStartup();

            refreshTimer.Elapsed += (s, e) => Dispatcher.Invoke(LoadIPInfo);
            refreshTimer.Start();

            LoadIPInfo();
            SetTheme();
        }

        private async void LoadIPInfo()
        {
            try
            {
                // Primary (fast + full info)
                string json = await client.GetStringAsync("https://ipwho.is/");
                var data = JsonDocument.Parse(json).RootElement;

                string ip = data.GetProperty("ip").GetString();
                string countryCode = data.GetProperty("country_code").GetString().ToLower();
                string city = data.GetProperty("city").GetString();
                string region = data.GetProperty("region").GetString();
                string isp = data.GetProperty("connection").GetProperty("isp").GetString();

                LocationText.Text = $"{city}, {region}";
                IspText.Text = isp;
                FlagImage.Source = new BitmapImage(
                    new Uri($"https://flagcdn.com/24x18/{countryCode}.png")
                );
                return;
            }
            catch { }

            // FALLBACK #1 → IP only (very reliable)
            try
            {
                string json = await client.GetStringAsync("https://api.myip.com/");
                var data = JsonDocument.Parse(json).RootElement;

                string ip = data.GetProperty("ip").GetString();
                string country = data.GetProperty("country").GetString();

                LocationText.Text = country;
                IspText.Text = ip;
                return;
            }
            catch { }

            // FALLBACK #2 → Last try
            try
            {
                string ip = await client.GetStringAsync("https://api.ipify.org");
                LocationText.Text = "Unknown Location";
                IspText.Text = ip;
                return;
            }
            catch { }

            // ☠ Final → Total offline
            LocationText.Text = "Offline";
            IspText.Text = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load IP info on startup
            LoadIPInfo();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MainBorder_MouseEnter(object sender, MouseEventArgs e) =>
            CloseButton.Visibility = Visibility.Visible;

        private void MainBorder_MouseLeave(object sender, MouseEventArgs e) =>
            CloseButton.Visibility = Visibility.Collapsed;

        private void CloseButton_Click(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();

        private void Refresh_Click(object sender, RoutedEventArgs e) => LoadIPInfo();

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            if (isDarkTheme)
            {
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(220, 240, 240, 240));
                LocationText.Foreground = Brushes.Black;
                IspText.Foreground = Brushes.Black;
            }
            else
            {
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(220, 25, 25, 25));
                LocationText.Foreground = Brushes.White;
                IspText.Foreground = Brushes.White;
            }
            isDarkTheme = !isDarkTheme;
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void SetTheme()
        {
            var isSystemDark = SystemParameters.WindowGlassColor.R < 128;
            if (isSystemDark)
            {
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(220, 240, 240, 240));
                LocationText.Foreground = Brushes.Black;
                IspText.Foreground = Brushes.Black;
                isDarkTheme = false;
            }
            else
            {
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(220, 25, 25, 25));
                LocationText.Foreground = Brushes.White;
                IspText.Foreground = Brushes.White;
                isDarkTheme = true;
            }
        }

        private void AddToStartup()
        {
            try
            {
                string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(runKey, true);
                string exePath = System.IO.Path.Combine(AppContext.BaseDirectory, "VpnWidget.exe");
                key.SetValue("VpnWidget", $"\"{exePath}\"");
            }
            catch { }
        }
    }
}
