using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomDRP
{
    public partial class MainWindow : Window
    {
        private string PathSave;
        private Config Config;

        private string Enable = "1";
        private List<string> Buttons = new();

        private AnimateManager animateManager;

        public MainWindow(string pathsave)
        {
            InitializeComponent();
            // Запуск анимационного менеджера.
            animateManager = new(60, this);
            animateManager.Start();

            // Анимация загрузки.
            Animate LoadAnimate = new(Load, 0, 30, true);
            animateManager.Play(LoadAnimate);

            PathSave = pathsave;

            Config = new Config();
            Dictionary<string, string> data = Config.Open(pathsave + "\\config.cfg");

            // Загрузка настроек из конфига.
            Details.Text = data["Details"];
            State.Text = data["State"];
            AppId.Text = data["AppID"];
            SmallBox.IsChecked = data["SmallIcon"] == "1";

            // Добавление кнопок.
            if (data["Buttons"] != ",;,")
            {
                int i = 0;
                foreach (string button in data["Buttons"].Split(";"))
                {
                    string[] b = button.Split(",");
                    if (string.IsNullOrEmpty(b[0])) { break; }


                    Buttons.Add(button);

                    if (i == 0) { Button1Key.Text = b[0]; Button1Value.Text = b[1]; }
                    else if (i == 1) { Button2Key.Text = b[0]; Button2Value.Text = b[1]; }
                    i++;
                }
            }

            ButtonsUpdate();
        }

        private void ButtonsAdd(TextBox Key, TextBox Value, int index)
        {
            string[] keyvalue = Buttons[index].Split(",");

            Key.Text = keyvalue[0];
            Value.Text = keyvalue[1];
        }

        private void ButtonsUpdate()
        {
            if (Buttons.Count < 2)
            {
                Button2Plus.Visibility = Visibility.Hidden; Button2Rect.Visibility = Visibility.Hidden;
                Delete2Button.Visibility = Visibility.Hidden;

                Button2Rect.Cursor = Cursors.Hand;

                Button2Key.IsEnabled = false; Button2Value.IsEnabled = false;

                Button2Value.Clear(); Button2Key.Clear();
            }

            if (Buttons.Count >= 1)
            {
                Button1Plus.Visibility = Visibility.Hidden;
                Button1Rect.Cursor = Cursors.Arrow;

                Button1Key.Visibility = Visibility.Visible; Button1Value.Visibility = Visibility.Visible;
                Button1Key.IsEnabled = true; Button1Value.IsEnabled = true;

                Delete1Button.Visibility = Visibility.Visible;

                this.Height = 588;

                Button2Plus.Visibility = Visibility.Visible;
                Button2Rect.Visibility = Visibility.Visible;

            }
            else
            {
                Button1Rect.Cursor = Cursors.Hand;
                Button1Plus.Visibility = Visibility.Visible;
                Delete1Button.Visibility = Visibility.Hidden;
                Button1Key.IsEnabled = false; Button1Value.IsEnabled = false;

                Button1Value.Clear(); Button1Key.Clear();

                this.Height = 555;
            }

            if (Buttons.Count == 2)
            {
                Button2Plus.Visibility = Visibility.Hidden;
                Button2Rect.Cursor = Cursors.Arrow;

                Button2Key.Visibility = Visibility.Visible; Button2Value.Visibility = Visibility.Visible;
                Delete2Button.Visibility = Visibility.Visible;
                Button2Key.IsEnabled = true; Button2Value.IsEnabled = true;
            }
        }

        // Меняет цвет при наведении.
        private static void ChangeBackground(TextBlock textblock, bool state)
        {
            if (state) { textblock.Background = new SolidColorBrush(Color.FromRgb(237, 66, 69)); }
            else { textblock.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)); }
        }

        private static bool IsDigint(string String)
        {
            for (int i = 0; i < String.Length; i++)
            {
                if (!char.IsDigit(String, i)) { return false; }
            }
            return true;
        }

        private void SaveConfig()
        {

            if (string.IsNullOrEmpty(State.Text)) { State.Text = " "; }
            if (string.IsNullOrEmpty(Details.Text)) { Details.Text = " "; }
            if (string.IsNullOrEmpty(AppId.Text) || !IsDigint(AppId.Text)) { AppId.Text = "957182426566754304"; }

            Dictionary<string, string> config = new Dictionary<string, string>()
            {
                ["AppID"] = AppId.Text,
                ["Details"] = Details.Text,
                ["State"] = State.Text,
                ["Buttons"] = $"{Button1Key.Text},{Button1Value.Text};{Button2Key.Text},{Button2Value.Text}",
                ["SmallIcon"] = (bool)SmallBox.IsChecked ? "1" : "0",
                ["Enable"] = Enable
            };

            Config.Save(PathSave + "\\config.cfg", config);

            // Если не будет ярлыка автозапуска, создаст новый.
            string authoRunFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                    "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\CustomDRP.lnk";
            if (Enable == "1")
            {
                WshShell wshShell = new();
                IWshShortcut Shortcut;

                Shortcut = (IWshShortcut)wshShell.CreateShortcut(authoRunFile);

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
        internal void CloseText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SaveConfig();

            this.Hide();

            try
            {
                Drp drp = new(PathSave);
                drp.Start();
            }
            catch (ArgumentException ex)
            {
                this.Show();
                MessageBox.Show($"Произошла ошибка при попытки запустить статус!\n{ex.Message}", "CustomDRP");
            }

        }

        internal void CloseText_MouseEnter(object sender, MouseEventArgs e) => ChangeBackground(CloseText, true);

        internal void CloseText_MouseLeave(object sender, MouseEventArgs e) => ChangeBackground(CloseText, false);
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

        private void GuideTextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Process.Start(new ProcessStartInfo("cmd", $"/c start https://github.com/aqur1n/CustomDRP/blob/master/README.md") { CreateNoWindow = true });

        private void Button1Rect_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Button1Plus.Visibility == Visibility.Visible)
            {
                Buttons.Add("Текст,Ссылка");

                ButtonsAdd(Button1Key, Button1Value, 0);
            }

            ButtonsUpdate();
        }

        private void Button2Rect_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Button2Plus.Visibility == Visibility.Visible)
            {
                Buttons.Add("Текст,Ссылка");

                ButtonsAdd(Button2Key, Button2Value, 1);
            }

            ButtonsUpdate();
        }

        private void Delete2Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Buttons.RemoveAt(1);

            Button2Key.Text = "";
            Button2Value.Text = "";

            ButtonsUpdate();
        }

        private void Delete1Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Button2Plus.Visibility == Visibility.Hidden)
            {
                Buttons[0] = Buttons[1];

                Button1Key.Text = Button2Key.Text;
                Button1Value.Text = Button2Value.Text;

                Buttons.RemoveAt(1);
            }
            else
            {
                Buttons.RemoveAt(0);

                Button1Key.Clear();
                Button1Value.Clear();
            }
            
            ButtonsUpdate();
        }
    }
}
