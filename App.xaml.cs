using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace CustomDRP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("CustomDRP");
            if (processes.Length >= 2)
            {
                int currentProcessId = Environment.ProcessId;

                foreach (Process process in processes)
                {
                    if (process.Id != currentProcessId) { process.Kill(); }
                }
            }


            string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\aqur1n-lab\\cdrp";

            if (!File.Exists(Path + "\\config.cfg"))
            { 
                Directory.CreateDirectory(Path);
                using (FileStream fstream = new(Path + "\\config.cfg", FileMode.OpenOrCreate))
                {
                    byte[] buffer = Encoding.Default.GetBytes("AppID=Айди приложения\nDetails=Текст\nState=Подпись\nButtons= \nSmallIcon=0\nEnable=0");
                    fstream.Write(buffer, 0, buffer.Length);
                }
            }

            if (e.Args.Length == 0)
            {
                MainWindow window = new(Path);
                window.Show();
            }
            else if (e.Args[0] == "--no-ui" || e.Args[0] == "--no-gui")
            {
                Drp drp = new(Path);
                drp.Start();
            }
            else
            {
                MessageWindow window = new("Запуск с неправильным(и) аргументом(ами)!");
                window.Show();
            }
        }
    }
}
