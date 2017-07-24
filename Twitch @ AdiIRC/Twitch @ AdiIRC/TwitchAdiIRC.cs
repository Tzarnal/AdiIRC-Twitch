using Timer = System.Threading.Timer;
using AdiIRCAPIv2.Interfaces;

namespace Twitch___AdiIRC
{
    public class TwitchAdiIrc : IPlugin
    {
        public string PluginDescription => "Provides simple additional features like emotes for twitch chat integration.";
        public string PluginAuthor => "Xesyto";
        public string PluginName => "Twitch @ AdiIRC";
        public string PluginVersion => "6";
        public string PluginEmail => "s.oudenaarden@gmail.com";

        private IPluginHost _host;

        private string _emoteDirectory;
        private Timer _topicTimer;
                
        private Settings _settings;        
        private SettingsForm _settingsForm;

        public void Initialize(IPluginHost host)
        {
            _host = host;
        }

        public void Dispose()
        {
            _settings.Save();        
        }        
    }
}
