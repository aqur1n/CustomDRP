using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomDRP
{
    /// <summary>
    /// Логика взаимодействия для MessageBox.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow(string Text, string Title)
        {
            InitializeComponent();

            TitleBlock.Text = Title;
            TextBlock.Text = Text;
        }

        public MessageWindow(string Text)
        {
            InitializeComponent();

            TextBlock.Text = Text;
        }
        

        public static void ChangeBackground(TextBlock textblock, bool state)
        {
            if (state) { textblock.Background = new SolidColorBrush(Color.FromRgb(237, 66, 69)); }
            else { textblock.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)); }
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();


        // Кнопка X
        private void CloseText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void CloseText_MouseEnter(object sender, MouseEventArgs e) => ChangeBackground(CloseText, true);

        private void CloseText_MouseLeave(object sender, MouseEventArgs e) => ChangeBackground(CloseText, false);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => this.WindowState = WindowState.Minimized;

        private void MinimizeText_MouseEnter(object sender, MouseEventArgs e) => ChangeBackground(MinimizeText, true);

        private void MinimizeText_MouseLeave(object sender, MouseEventArgs e) => ChangeBackground(MinimizeText, false);

        private void Window_StateChanged(object sender, EventArgs e) { if (WindowState == WindowState.Normal) { this.Topmost = true; this.Topmost = false; } }
    }
}
