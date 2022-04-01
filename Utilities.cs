

using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CustomDRP
{
    internal class Position
    {
        public int X;
        public int Y;

        public Position(int x, int y)
        { 
            X = x;
            Y = y;
        }

        public static Position New(int x, int y)
        {
            return new Position(x, y);
        }

        public static Position Zero()
        {
            return new Position(0,0);
        }
    }

    internal class Effect
    {
        //                             скорость, кон. коорд., нынеш коорд
        public static float Linear(float x, float y, float z)
        {
            //MessageBox.Show($"x - {x}, y - {y}, z - {z}");
            if (Math.Abs(z) < Math.Abs(y)) { return y > z ? z + x : z - x; }
            else { return y; }
        }

        public static float Racing(float x, float y, float z)
        {
            return y > z ? 2 * x : z - 2 * x;
        }
    }

    internal class Other
    {
        public static bool IsDigit(string String)
        {
            for (int i = 0; i < String.Length; i++)
            {
                if (!char.IsDigit(String, i)) { return false; }
            }
            return true;
        }
        public static float Limit(float x, float z, float y)
        {
            if (x <= y && x >= z) { return x; }
            else if (x > y) { return y; }
            else { return z; }
        }

        public static float Limit(float x, float z)
        {
            if (x >= z) { return x; }
            else { return z; }
        }

        public static void CreateShotcut()
        {
            string authoRunFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                        "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\CustomDRP.lnk";
            
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
}
