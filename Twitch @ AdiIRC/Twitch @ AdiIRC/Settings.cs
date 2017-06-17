using System;
using System.IO;
using Newtonsoft.Json;

namespace Twitch___AdiIRC
{
    public class Settings
    {
        public string Path;

        public bool ShowTimeouts=true;
        public bool ShowCheers=true;
        public bool ShowSubs=true;
        public bool ShowBadges=true;
        public bool AutoComplete = true;

        public void Save()
        {
            var data = JsonConvert.SerializeObject(this);
            var configFolder = System.IO.Path.GetDirectoryName(Path);

            if (!string.IsNullOrWhiteSpace(configFolder) && !Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            try
            {
                File.WriteAllText(Path, data);
            }
            catch (Exception)
            {                
            }

        }

        public static Settings Load(string path)
        {            
            var data = File.ReadAllText(path);

            var settings = JsonConvert.DeserializeObject<Settings>(data);
            settings.Path = path;

            return settings;
        }
    }
}
