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
                        Label = b[0],
                        Url = b[1]
                    };

                    count++;
                }
            }
            else { Buttons = new Button[1]; }

            Client.SetPresence(new RichPresence()
            {
                Assets = assets,
                Details = data["Details"],
                State = data["State"],
                Buttons = Buttons

            });
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
