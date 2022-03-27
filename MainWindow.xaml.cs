using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using IWshRuntimeLibrary;

namespace CustomDRP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string PathSave;
        Config Config;
        string Enable = "1";

        public MainWindow(string pathsave)
        {
            InitializeComponent();
            PathSave = pathsave;

            Config = new Config();
            Dictionary<string, string> data = Config.Open(pathsave + "\\config.cfg");

            Details.Text = data["Details"];
            State.Text = data["State"];
            AppId.Text = data["AppID"];
            SmallBox.IsChecked = data["SmallIcon"] == "1";
        }

        // Меняет цвет при наведении.
        public static void ChangeBackground(TextBlock textblock, bool state)
        {
            if (state) { textblock.Background = new SolidColorBrush(Color.FromRgb(237, 66, 69)); }
            else { textblock.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)); }
        }

        public static bool IsDigint(string String)
        {
            for (int i = 0; i < String.Length; i++)
            {
                if (!char.IsDigit(String, i)) { return false; }
            }
            return true;
        }

        public void SaveConfig()
        {

            if (string.IsNullOrEmpty(State.Text)) { State.Text = " "; }
            if (string.IsNullOrEmpty(Details.Text)) { Details.Text = " "; }
            if (string.IsNullOrEmpty(AppId.Text) || !IsDigint(AppId.Text)) { AppId.Text = "957182426566754304"; }

            Dictionary<string, string> config = new Dictionary<string, string>()
            {
                ["AppID"] = AppId.Text,
                ["Details"] = Details.Text,
                ["State"] = State.Text,
                ["Buttons"] = " ",
                ["SmallIcon"] = (bool)SmallBox.IsChecked ? "1" : "0" ,
                ["Enable"] = Enable
            };

            Config.Save(PathSave + "\\config.cfg", config);

            // Если не будет ярлыка автозапуска, создаст новый.
            string authoRunFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                    "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\CustomDRP.lnk";
            if (Enable == "1" && !System.IO.File.Exists(authoRunFile))
            {
                WshShell wshShell = new WshShell();

                IWshShortcut Shortcut = (IWshShortcut)wshShell.
                    CreateShortcut(authoRunFile);

                Shortcut.TargetPath = Environment.CurrentDirectory + "\\CustomDRP.exe";
                Shortcut.Arguments = "--no-ui";
                Shortcut.WorkingDirectory = Environment.CurrentDirectory;
                Shortcut.IconLocation = Environment.CurrentDirectory + "\\CustomDRP.exe";

                Shortcut.Save();
            }
        }

        // Вызывается при нажатии мыши на пустом окне.
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();


        // Кнопка X
        private void CloseText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SaveConfig();

            this.Close();
            Drp drp = new(PathSave);
            drp.Start();
        }

        private void CloseText_MouseEnter(object sender, MouseEventArgs e) => ChangeBackground(CloseText, true);

        private void CloseText_MouseLeave(object sender, MouseEventArgs e) => ChangeBackground(CloseText, false);
        // ~~~~~~~~

        // Кнопка для сворачивании ( - ).
        private void MinimizeText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => this.WindowState = WindowState.Minimized;

        private void MinimizeText_MouseEnter(object sender, MouseEventArgs e) => ChangeBackground(MinimizeText, true);

        private void MinimizeText_MouseLeave(object sender, MouseEventArgs e) => ChangeBackground(MinimizeText, false);
        // ~~~~~~~~

        private void Hyperlink_Click(object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("cmd", $"/c start https://discord.gg/HbtSHsWv4b") { CreateNoWindow = true });

        // Если окно разварачивают, оно перемещается на передний план.
        private void Window_StateChanged(object sender, EventArgs e) { if (WindowState == WindowState.Normal) { this.Topmost = true; this.Topmost = false; } }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.FocusedElement is TextBox)
            {
                Keyboard.ClearFocus();
            }
        }

        private void StopTextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Enable = "0";
            SaveConfig();

            Environment.Exit(0);
        }

        private void StopTextBlock_MouseEnter(object sender, MouseEventArgs e) { StopTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(7, 129, 177)); }

        private void StopTextBlock_MouseLeave(object sender, MouseEventArgs e) { StopTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(175, 177, 180)); }

        private void GuideTextBlock_MouseEnter(object sender, MouseEventArgs e) { GuideTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(7, 129, 177)); }

        private void GuideTextBlock_MouseLeave(object sender, MouseEventArgs e) { GuideTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(175, 177, 180)); }

        private void GuideTextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Process.Start(new ProcessStartInfo("cmd", $"/c start https://discord.gg/HbtSHsWv4b") { CreateNoWindow = true });
    }
}
