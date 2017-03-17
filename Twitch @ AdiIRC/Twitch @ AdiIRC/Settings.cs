using System;
using System.IO;
using Newtonsoft.Json;

namespace Twitch___AdiIRC
{
    public class Settings
    {
        public static string DataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                        @"\AdiIRC\Plugins\TwitchConfig\";
        public static string DataFileName = "Config.json";

        public bool ShowTimeouts=true;
        public bool ShowCheers=true;
        public bool ShowSubs=true;
        public bool ShowBadges=true;
        
        public static string FullPath
        {
            get { return DataPath + DataFileName; }

        }

        public void Save()
        {
            var data = JsonConvert.SerializeObject(this);

            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }

            try
            {
                File.WriteAllText(FullPath, data);
            }
            catch (Exception)
            {                
            }

        }

        public static Settings Load()
        {
            var data = File.ReadAllText(FullPath);
            return JsonConvert.DeserializeObject<Settings>(data);

        }
    }
}
