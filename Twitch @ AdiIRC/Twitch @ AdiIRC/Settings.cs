using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Twitch___AdiIRC
{
    public class Settings
    {
        public string Path;

        public bool ShowTimeouts = true;
        public bool ShowCheers = true;
        public bool ShowSubs = true;
        public bool ShowBadges = true;
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
                MessageBox.Show($"Could not write to AdiIRC@Twich's config file ({Path})");
            }
        }

        public static Settings Load(string path)
        {
            string data;

            try
            {
                data = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Could not read from AdiIRC@Twich's config file. Using Default Values.");
                return new Settings();
            }

            Settings settings;

            try
            {
                settings = JsonConvert.DeserializeObject<Settings>(data);
            }
            catch (Exception)
            {
                MessageBox.Show($"Could not understand AdiIRC@Twich's config file. Using Default Values.");
                return new Settings();
            }

            settings.Path = path;
            return settings;
        }
    }
}
