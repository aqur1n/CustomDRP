using System.Collections.Generic;
using System.IO;

namespace CustomDRP
{
    internal class Config
    {
        public Dictionary<string, string> config;
        public Dictionary<string, string> Open(string path)
        {
            config = new Dictionary<string, string>();

            foreach (string line in File.ReadAllLines(path))
            {
                string[] parts = line.Split('=');
                if (parts.Length > 1) { config[parts[0]] = parts[1]; }
            }

            return config;
        }

        public void Save(string path, Dictionary<string, string> config)
        {
            string file = "";

            foreach (var data in config)
            {
                file += $"{data.Key}={data.Value}\n";
            }

            using (StreamWriter sw = new(path, false))
            {
                sw.WriteLine(file);
                sw.Close();
            }

        }
        public void Save(string path)
        {
            this.Save(path, this.config);
        }

    }
}
