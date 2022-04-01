using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CustomDRP
{
    internal class Drp
    {
        public DiscordRpcClient Client;
        Dictionary<string, string> data;
        Config config;

        public Drp(string pathsave)
        {
            config = new Config();

            data = config.Open(pathsave + "\\config.cfg");

            // Проверяет, включен ли.
            if (data["Enable"] == "0") { Environment.Exit(0); }

            Client = new DiscordRpcClient(data["AppID"])
            {
                Logger = new ConsoleLogger() { Level = LogLevel.Warning }
            };

            Client.Initialize();

            Assets assets = new()
            {
                LargeImageKey = "1"
            };
            if (data["SmallIcon"] == "1") { assets.SmallImageKey = "2"; }

            Button[] Buttons;

            if (data["Buttons"] != ",;,")
            {
                int count = 0;
                string[] buttons = data["Buttons"].Split(";");

                if (buttons[1] != ",") { Buttons = new Button[2]; }
                else { Buttons = new Button[1]; }

                foreach (string button in buttons)
                {
                    string[] b = button.Split(",");

                    if (Buttons.Length - 1 >= count) Buttons[count] = new Button
                    {
                        Label = string.IsNullOrEmpty(b[0]) ? "Создано CustomDRP" : b[0],
                        Url = string.IsNullOrEmpty(b[1]) ? "https://discord.gg/HbtSHsWv4b" : b[1]
                    };

                    count++;
                }
            }
            else { Buttons = Array.Empty<Button>(); }

            RichPresence rp = new()
            {
                Assets = assets,
                Details = data["Details"],
                State = data["State"]
            };

            if (Buttons.Length != 0) { rp.Buttons = Buttons; }

            Client.SetPresence(rp);
        }
        public void Start()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public void Deinitialize()
        {
            Client.Deinitialize();
        }
    }
}
