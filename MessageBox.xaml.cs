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
        MainWindow window;

        public MessageWindow(MainWindow window, string Text)
        {
            this.window = window;
            window.CloseText.MouseLeftButtonUp -= window.CloseText_MouseLeftButtonUp;
            window.CloseText.Cursor = Cursors.Arrow;

            window.CloseText.MouseEnter -= window.CloseText_MouseEnter;
            window.CloseText.MouseLeave -= window.CloseText_MouseLeave;

            InitializeComponent();

            TextBlock.Text = Text;



            this.Show();
        }

        public MessageWindow(string Text)
        {
            InitializeComponent();

            TextBlock.Text = Text;

            this.Show();
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
            if (window is not null)
            {
                window.CloseText.MouseLeftButtonUp += window.CloseText_MouseLeftButtonUp;
                window.CloseText.Cursor = Cursors.Hand;

                window.CloseText.MouseEnter += window.CloseText_MouseEnter;
                window.CloseText.MouseLeave += window.CloseText_MouseLeave;
            }

            this.Close();
        }

        private void CloseText_MouseEnter(object sender, MouseEventArgs e) => ChangeBackground(CloseText, true);

        private void CloseText_MouseLeave(object sender, MouseEventArgs e) => ChangeBackground(CloseText, false);

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (window is not null)
            {
                window.CloseText.MouseLeftButtonUp += window.CloseText_MouseLeftButtonUp;
                window.CloseText.Cursor = Cursors.Hand;

                window.CloseText.MouseEnter += window.CloseText_MouseEnter;
                window.CloseText.MouseLeave += window.CloseText_MouseLeave;
            }

            this.Close();
        }

        private void Window_StateChanged(object sender, EventArgs e) { if (WindowState == WindowState.Normal) { this.Topmost = true; this.Topmost = false; } }
    }
}
