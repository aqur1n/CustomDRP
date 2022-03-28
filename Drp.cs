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
        string FileSave;

        public void Update()
        {
            this.data = config.Open(FileSave);
        }

        public Drp(string pathsave)
        {
            config = new Config();
            FileSave = pathsave + "\\config.cfg";

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

            Button[] Buttons = new Button[2];

            if (data["Buttons"] != " " && data["Buttons"] != ",;,")
            {
                int count = 0;
                foreach (string button in data["Buttons"].Split(";"))
                {
                    string[] b = button.Split(",");

                    Buttons[count] = new Button
                    {
                        Label = b[0],
                        Url = b[1]
                    };

                    count++;
                }
            }

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
